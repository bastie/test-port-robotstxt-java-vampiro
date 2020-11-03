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

namespace com.google.search.robotstxt
{

/**
 * Class implementing matching logic based on directives priorities those calculation is delegated
 * to a {@link MatchingStrategy} class.
 */
public class RobotsMatcher : Matcher {
  //FIXIT: later logging
  //private static readonly FluentLogger logger = FluentLogger.forEnclosingClass();

  /** Class containing current match priorities */
  private class Match {
    /** Priority based on agent-specific rules */
    private int prioritySpecific = 0;
    /** Priority based on global wildcard (*) rules */
    private int priorityGlobal = 0;

    internal void updateSpecific(int priority) {
      prioritySpecific = java.lang.Math.max(prioritySpecific, priority);
    }

    internal void updateGlobal(int priority) {
      priorityGlobal = java.lang.Math.max(priorityGlobal, priority);
    }

    public int getPrioritySpecific() {
      return prioritySpecific;
    }

    public int getPriorityGlobal() {
      return priorityGlobal;
    }

    public void resetGlobal() {
      priorityGlobal = 0;
    }
  }

  private RobotsContents robotsContents;
  private MatchingStrategy matchingStrategy = new RobotsLongestMatchStrategy();

  public RobotsMatcher(RobotsContents robotsContents) {
    this.robotsContents = robotsContents;
  }

  /** Used to extract contents for testing purposes. */
  internal RobotsContents getRobotsContents() {
    return robotsContents;
  }

  private static String getPath(String url) {
    java.net.URL parsedUrl;
    try {
      parsedUrl = new java.net.URL(url);
    } catch (java.net.MalformedURLException e) {
      //FIXIT: later logging
      //logger.atWarning().log("Malformed URL: \"%s\", replaced with \"/\"", url);
      return "/";
    }
    String path = parsedUrl.getPath();
    String args = parsedUrl.getQuery();
    if (args != null) {
      path += "?" + args;
    }

    return path;
  }

  /**
   * Computes {@link Match} priorities for ALLOW and DISALLOW verdicts. Rules are considered
   * effective if at least one user agent is listed in "user-agent" directives or applied globally
   * (if global rules are not ignored).
   *
   * @param userAgents list of interested user agents
   * @param path target path
   * @param ignoreGlobal global rules will not be considered if set to {@code true}
   * @return pair of {@link Match} representing ALLOW and DISALLOW priorities respectively
   */
  private java.util.MapNS.Entry<Match, Match> computeMatchPriorities(
      java.util.List<String> userAgents, String path, bool ignoreGlobal) {
    Match allow = new Match();
    Match disallow = new Match();
    bool foundSpecificGroup = false;

    java.util.Iterator<RobotsContents.Group> iter1 = robotsContents.getGroups().iterator();
    while (iter1.hasNext()) {
      RobotsContents.Group group = iter1.next();
    //foreach (RobotsContents.Group group in robotsContents.getGroups()) {
      bool isSpecificGroup = false;
      for (int i = 0; i < userAgents.size() && !isSpecificGroup; i++) { // userAgents.stream()
        java.util.Iterator<String> iter3 = group.getUserAgents().iterator();//group.getUserAgents().stream()
        while (iter3.hasNext()) {
          String next3 = iter3.next().ToLower();//userAgent::equalsIgnoreCase
          if (userAgents.get(i).ToLower().Equals(next3)) { // anyMatch/anyMatch
            isSpecificGroup = true;
          }
        }
      }
      /* see lines before - better with LINQ, maybe later 
      bool isSpecificGroup =
          userAgents.stream()
              .anyMatch(
                  userAgent ->
                      group.getUserAgents().stream().anyMatch(userAgent::equalsIgnoreCase));
      */
      foundSpecificGroup |= isSpecificGroup;
      if (!isSpecificGroup && (ignoreGlobal || !group.isGlobal())) {
        continue;
      }

      java.util.Iterator<RobotsContents.Group.Rule> iter2 = group.getRules().iterator();
      while (iter2.hasNext()) {
         RobotsContents.Group.Rule rule = iter2.next();
      //foreach (RobotsContents.Group.Rule rule in group.getRules()) {
        switch (rule.getDirectiveType()) {
          case Parser.DirectiveType.ALLOW:
            {
              int priority =
                  matchingStrategy.matchAllowPriority(path, rule.getDirectiveValue());
              if (isSpecificGroup) {
                allow.updateSpecific(priority);
              }
              if (!ignoreGlobal && group.isGlobal()) {
                allow.updateGlobal(priority);
              }
              break;
            }
          case Parser.DirectiveType.DISALLOW:
            {
              int priority =
                  matchingStrategy.matchDisallowPriority(path, rule.getDirectiveValue());
              if (isSpecificGroup) {
                disallow.updateSpecific(priority);
              }
              if (!ignoreGlobal && group.isGlobal()) {
                disallow.updateGlobal(priority);
              }
              break;
            }
          case Parser.DirectiveType.SITEMAP:
          case Parser.DirectiveType.UNKNOWN:
          case Parser.DirectiveType.USER_AGENT:
            break;
        }
      }
    }

    // If there is at least one group specific for current agents, global groups should be
    // disregarded.
    if (foundSpecificGroup) {
      allow.resetGlobal();
      disallow.resetGlobal();
    }

    return java.util.Map<Match,Match>.entry(allow, disallow);
  }

  private java.util.MapNS.Entry<Match, Match> computeMatchPriorities(
      java.util.List<String> userAgents, String path) {
    return computeMatchPriorities(userAgents, path, false);
  }

  /**
   * Return {@code true} iff verdict must be ALLOW based on ALLOW and DISALLOW priorities.
   *
   * @param allow ALLOW priorities
   * @param disallow DISALLOW priorities
   * @return match verdict
   */
  private static bool allowVerdict(Match allow, Match disallow) {
    if (allow.getPrioritySpecific() > 0 || disallow.getPrioritySpecific() > 0) {
      return allow.getPrioritySpecific() >= disallow.getPrioritySpecific();
    }

    if (allow.getPriorityGlobal() > 0 || disallow.getPriorityGlobal() > 0) {
      return allow.getPriorityGlobal() >= disallow.getPriorityGlobal();
    }

    return true;
  }

  public bool allowedByRobots(java.util.List<String> userAgents, String url) {
    String path = getPath(url);
    java.util.MapNS.Entry<Match, Match> matches = computeMatchPriorities(userAgents, path);
    return allowVerdict(matches.getKey(), matches.getValue());
  }

  public bool singleAgentAllowedByRobots(String userAgent, String url) {
    return allowedByRobots(java.util.Collections<String>.singletonList(userAgent), url);
  }

  public bool ignoreGlobalAllowedByRobots(java.util.List<String> userAgents, String url) {
    String path = getPath(url);
    java.util.MapNS.Entry<Match, Match> matches = computeMatchPriorities(userAgents, path, true);
    return allowVerdict(matches.getKey(), matches.getValue());
  }
}
}