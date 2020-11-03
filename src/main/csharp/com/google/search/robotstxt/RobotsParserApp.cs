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
//using com.google.common.io;

using picocli = biz.ritter.robotstxt.facade.picocli;


namespace com.google.search.robotstxt
{
    // original project has external dependency to https://github.com/remkop/picocli for only three parameters
    // original project has external depandency to https://github.com/google/guava for only one call ByteStreams.toByteArray


    //@CommandLine.Command(
    //    name = "robotsParser",
    //    description =
    //        "Parses and matches given agents against given robots.txt to determine "
    //            + "whether any agent is allowed to visit given URL.",
    //    exitCodeOnExecutionException = 2,
    //    exitCodeOnInvalidInput = 3)
    public class RobotsParserApp : java.util.concurrent.Callable<java.lang.Integer>
    {
        static void Main(string[] args)
        {
            int exitCode = new picocli.CommandLine<java.lang.Integer>(new RobotsParserApp()).execute(args);
            java.lang.SystemJ.exit(exitCode);
        }
        /** robots.txt file path. */
        //@CommandLine.Option(names = {"-f", "--file"})
        private String robotsTxtPath;

        /** Interested user-agents. */
        //@CommandLine.Option(
        //    names = {"-a", "--agent"},
        //    required = true)
        private java.util.List<String> agents;

        /** Target URL to match. */
        //@CommandLine.Option(
        //    names = {"-u", "--url"},
        //    required = true)
        private String url;

        private byte[] readRobotsTxt() {//throws ParseException {
            try {
            if (java.util.Objects.isNull(robotsTxtPath)) {
                // Reading from stdin
                return com.google.commons.io.ByteStreams.toByteArray(java.lang.SystemJ.inJ);


            } else {
                // Reading from file
                return java.nio.file.Files.readAllBytes(java.nio.file.Path.of(robotsTxtPath));
            }
            }
            catch (java.lang.Exception ex) when (
                ex is java.io.UncheckedIOException
                || ex is java.io.IOException
                || ex is java.nio.file.InvalidPathException
            ) { //catch (UncheckedIOException | IOException | InvalidPathException e) {
            throw new ParseException("Failed to read robots.txt file.", ex);
            }
        }

        private static void logError(java.lang.Exception e) {
            java.lang.SystemJ.outJ.println("ERROR: " + e.getMessage());
            //FIXIT: logger later
            //logger.atInfo().withCause(e).log("Stack trace:");
        }

        /**
        * Parses given robots.txt file and performs matching process.
        *
        * @return {@code 0} if any of user-agents is allowed to crawl given URL and {@code 1} otherwise.
        */
        public java.lang.Integer call() {
            byte[] robotsTxtContents;
            try {
            robotsTxtContents = readRobotsTxt();
            } catch (ParseException e) {
            logError(e);
            return 2;
            }

            Parser parser = new RobotsParser(new RobotsParseHandler());
            RobotsMatcher matcher = (RobotsMatcher) parser.parse(robotsTxtContents);

            bool parseResult;
            parseResult = matcher.allowedByRobots(agents, url);

            if (parseResult) {
            java.lang.SystemJ.outJ.println("ALLOWED");
            return 0;
            } else {
            java.lang.SystemJ.outJ.println("DISALLOWED");
            return 1;
            }
        }
    }
}
