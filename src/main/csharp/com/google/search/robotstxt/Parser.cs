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

namespace com.google.search.robotstxt{

  /**
  * Abstract parser. All parser implementations must extend it. This class extensions are expected to
  * provide tokenizer logic while parsing logic is delegated to a {@link ParseHandler} class.
  */
  public abstract class Parser {
    public enum DirectiveType {
      USER_AGENT,
      ALLOW,
      DISALLOW,
      SITEMAP,
      UNKNOWN
    }


    protected ParseHandler parseHandler;

    /**
    * Parser must follow specific {@link ParseHandler} rules in order to parse. Thus it requires an
    * instance of it upon creation.
    *
    * @param parseHandler handler to follow during parsing process.
    */
    protected Parser(ParseHandler parseHandler) {
      this.parseHandler = parseHandler;
    }

    /**
    * Method to parse robots.txt file into a matcher.
    *
    * @param robotsTxtBodyBytes body of robots.txt file to parse
    * @return matcher representing given robots.txt file
    */
    internal abstract Matcher parse(byte[] robotsTxtBodyBytes);
  }
}

      public static class DirectiveTypeExtension {
            public static com.google.search.robotstxt.Parser.DirectiveType 
                   valueOf(this com.google.search.robotstxt.Parser.DirectiveType dType, String name)
        {
          return DirectiveTypeExtension.valueOf(name);
        }
            public static com.google.search.robotstxt.Parser.DirectiveType 
                   valueOf(String name)
        {
            if (null == name) throw new java.lang.NullPointerException ();
            switch (name) {
              case "USER_AGENT" : return com.google.search.robotstxt.Parser.DirectiveType.USER_AGENT;
              case "ALLOW"      : return com.google.search.robotstxt.Parser.DirectiveType.ALLOW;
              case "DISALLOW"   : return com.google.search.robotstxt.Parser.DirectiveType.DISALLOW;
              case "SITEMAP"    : return com.google.search.robotstxt.Parser.DirectiveType.SITEMAP;
              case "UNKNOWN"    : return com.google.search.robotstxt.Parser.DirectiveType.UNKNOWN;
            }
            throw new java.lang.IllegalArgumentException (String.Format("Unknown {0} Parser.DirectiveType name.",name));
        }
    }


