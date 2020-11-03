// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// Ported 2020 Sebastian Ritter

using System;
using java = biz.ritter.javapi;

//using com.google.common.flogger;


namespace com.google.search.robotstxt
{

/** Implementation of parsing strategy used in robots.txt parsing. */
public class RobotsParseHandler : ParseHandler {
  //FIXIT: logging later
  //private static final FluentLogger logger = FluentLogger.forEnclosingClass();

  protected RobotsContents robotsContents;
  private RobotsContents.Group currentGroup;
  private bool foundContent;

  public void handleStart() {
    robotsContents = new RobotsContents();
    currentGroup = new RobotsContents.Group();
    foundContent = false;
  }

  private void flushCompleteGroup(bool createNew) {
    robotsContents.addGroup(currentGroup);
    if (createNew) {
      currentGroup = new RobotsContents.Group();
    }
  }

  public void handleEnd() {
    flushCompleteGroup(false);
  }

  private void handleUserAgent(String value) {
    if (foundContent) {
      flushCompleteGroup(true);
      foundContent = false;
    }
    currentGroup.addUserAgent(value);
  }

  private static bool isHexChar(byte b) {
    return java.lang.Character.isDigit((char)b) || ('a' <= (char)b && (char)b <= 'f') || ('A' <= (char)b && (char)b <= 'F');
  }

  /**
   * Canonicalize paths: escape characters outside of US-ASCII charset (e.g. /SanJoséSellers ==>
   * /Sanjos%C3%A9Sellers) and normalize escape-characters (e.g. %aa ==> %AA)
   *
   * @param path Path to canonicalize.
   * @return escaped and normalized path
   */
  private static String maybeEscapePattern(String path) {
    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(path);// path.getBytes(java.nio.charset.StandardCharsets.UTF_8);

    int unescapedCount = 0;
    bool notCapitalized = false;

    // Check if any changes required
    for (int i = 0; i < bytes.Length; i++) {
      if (i < bytes.Length - 2
          && bytes[i] == '%'
          && isHexChar(bytes[i + 1])
          && isHexChar(bytes[i + 2])) {
        if (java.lang.Character.isLowerCase((char)bytes[i + 1]) || java.lang.Character.isLowerCase((char)bytes[i + 2])) {
          notCapitalized = true;
        }
        i += 2;
      } else if ((bytes[i] & 0x80) != 0) {
        unescapedCount++;
      }
    }

    // Return if no changes needed
    if (unescapedCount == 0 && !notCapitalized) {
      return path;
    }

    java.lang.StringBuilder stringBuilder = new java.lang.StringBuilder();
    for (int i = 0; i < bytes.Length; i++) {
      if (i < bytes.Length - 2
          && bytes[i] == '%'
          && isHexChar(bytes[i + 1])
          && isHexChar(bytes[i + 2])) {
        stringBuilder.append((char) bytes[i++]);
        stringBuilder.append((char) java.lang.Character.toUpperCase((char)bytes[i++]));
        stringBuilder.append((char) java.lang.Character.toUpperCase((char)bytes[i]));
      } else if ((bytes[i] & 0x80) != 0) {
        stringBuilder.append('%');
        stringBuilder.append(java.lang.Integer.toHexString((bytes[i] >> 4) & 0xf).toUpperCase());
        stringBuilder.append(java.lang.Integer.toHexString(bytes[i] & 0xf).toUpperCase());
      } else {
        stringBuilder.append((char) bytes[i]);
      }
    }
    return stringBuilder.toString();
  }

  public void handleDirective(
      Parser.DirectiveType directiveType, String directiveValue) {
    switch (directiveType) {
      case Parser.DirectiveType.USER_AGENT:
        {
          handleUserAgent(directiveValue);
          break;
        }
      case Parser.DirectiveType.ALLOW:
      case Parser.DirectiveType.DISALLOW:
        {
          foundContent = true;
          if (currentGroup.isGlobal() || currentGroup.getUserAgents().size() > 0) {
            String path = maybeEscapePattern(directiveValue);
            currentGroup.addRule(directiveType, path);

            if (directiveType == Parser.DirectiveType.ALLOW) {
              // Google-specific optimization: 'index.htm' and 'index.html' are normalized to '/'.
              int slashPos = path.lastIndexOf('/');

              if (slashPos != -1) {
                String fileName = path.substring(slashPos + 1);
                if ("index.htm".equals(fileName) || "index.html".equals(fileName)) {
                  String normalizedPath = path.substring(0, slashPos + 1) + '$';

                  if (!currentGroup.hasRule(Parser.DirectiveType.ALLOW, normalizedPath)) {
                    //FITIT: logging later
                    //logger.atInfo().log(
                    //    "Allowing normalized path: \"%s\" -> \"%s\"",
                    //    directiveValue, normalizedPath);
                    currentGroup.addRule(Parser.DirectiveType.ALLOW, normalizedPath);
                  }
                }
              }
            }
          }
          break;
        }
      case Parser.DirectiveType.SITEMAP:
      case Parser.DirectiveType.UNKNOWN:
        {
          foundContent = true;
          break;
        }
    }
  }

  public Matcher compute() {
    return new RobotsMatcher(robotsContents);
  }
}
}