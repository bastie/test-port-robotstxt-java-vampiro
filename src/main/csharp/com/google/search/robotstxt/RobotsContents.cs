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
    public class RobotsContents
    {
  //FIXME: Logging later
  //private static readonly FluentLogger logger = FluentLogger.forEnclosingClass();
  /**
   * Representation of robots.txt group of rules: multiple user-agents to which multiple rules are
   * applied.
   */
  public class Group {
    /** Representation of robots.txt rule: pair of directive and value. */
    public class Rule {
      private readonly Parser.DirectiveType directiveType;
      private readonly String directiveValue;

      internal Rule(Parser.DirectiveType newDirectiveType, String newDirectiveValue) {
        this.directiveType = newDirectiveType;
        this.directiveValue = newDirectiveValue;
      }

      public Parser.DirectiveType getDirectiveType() {
        return directiveType;
      }

      public String getDirectiveValue() {
        return directiveValue;
      }

      public bool equals(Object obj) {
        if (this == obj) return true;
        if (obj == null || this.getClass() != obj.getClass()) return false;
        Rule other = (Rule) obj;
        return java.util.Objects.equals(directiveType, other.directiveType)
            && java.util.Objects.equals(directiveValue, other.directiveValue);
      }

      public int hashCode() {
        return java.util.Objects.hash(directiveType, directiveValue);
      }
    }

    private readonly java.util.Set<String> userAgents;
    private readonly java.util.Set<Rule> rules;
    private bool global = false;

    internal Group() {
      userAgents = new java.util.HashSet<String>();
      rules = new java.util.HashSet<Rule>();
    }

    // Intended to be used from tests only.
    internal Group(java.util.List<String> userAgents, java.util.List<Rule> rules) :
      this(userAgents, rules, false){
    }

    // Intended to be used from tests only.
    internal Group(java.util.List<String> userAgents, java.util.List<Rule> rules, bool global) {
      this.userAgents = new java.util.HashSet<String>(userAgents);
      this.rules = new java.util.HashSet<Rule>(rules);
      this.global = global;
    }

    internal void addUserAgent(String userAgent) {
      // Google-specific optimization: a '*' followed by space and more characters
      // in a user-agent record is still regarded a global rule.
      if (userAgent.length() >= 1
          && userAgent.charAt(0) == '*'
          && (userAgent.length() == 1 || java.lang.Character.isWhitespace(userAgent.charAt(1)))) {

        if (userAgent.length() > 1 && java.lang.Character.isWhitespace(userAgent.charAt(1))) {
          //FIXME: 
          //logger.atInfo().log("Assuming \"%s\" user-agent as \"*\"", userAgent);
        }

        global = true;
      } else {
        int end = 0;
        for (; end < userAgent.length(); end++) {
          char ch = userAgent.charAt(end);
          if (!java.lang.Character.isAlphabetic(ch) && ch != '-' && ch != '_') {
            break;
          }
        }
        userAgents.add(userAgent.substring(0, end));
      }
    }

    internal void addRule(Parser.DirectiveType directiveType, String directiveValue) {
      rules.add(new Rule(directiveType, directiveValue));
    }

    internal bool hasRule(Parser.DirectiveType directiveType, String directiveValue) {
      return rules.contains(new Rule(directiveType, directiveValue));
    }

    public java.util.Set<String> getUserAgents() {
      return userAgents;
    }

    public java.util.Set<Rule> getRules() {
      return rules;
    }

    public bool isGlobal() {
      return global;
    }

    public bool equals(Object obj) {
      if (this == obj) return true;
      if (obj == null || this.getClass() != obj.getClass()) return false;
      Group other = (Group) obj;
      return java.util.Objects.equals(userAgents, other.userAgents)
          && java.util.Objects.equals(rules, other.rules)
          && java.util.Objects.equals(global, other.global);
    }

    public int hashCode() {
      return java.util.Objects.hash(userAgents, rules);
    }
  }

  private readonly java.util.List<Group> groups;

  internal RobotsContents() {
    groups = new java.util.ArrayList<Group>();
  }

  public RobotsContents(java.util.List<Group> newGroups) {
    this.groups = newGroups;
  }

  internal void addGroup(Group group) {
    groups.add(group);
  }

  public java.util.List<Group> getGroups() {
    return groups;
      }
    }
}