# Google Robots.txt Parser and Matcher Library in C#

This project aims to implement the robots.txt parser and matcher in C#. It is
based on the [Java implementation](https://github.com/google/robotstxt-java).

## Status
This project is compileable with ''Just Another Vampire Api v0.11 or higher''.

## About the library

The Robots Exclusion Protocol (REP) is a standard that enables website owners
to control which URLs may be accessed by automated clients (i.e. crawlers)
through a simple text file with a specific syntax. It's one of the basic
building blocks of the internet as we know it and what allows search engines
to operate.

Because the REP was only a de-facto standard for the past 25 years, different
implementers implement parsing of robots.txt slightly differently, leading to
confusion. This project aims to fix that by releasing the parser that Google
uses.

The library is a C# port of 
[Java parser and matcher](https://github.com/google/robotstxt-java) which is a
slightly modified production code used by Googlebot, Google's crawler. The
library is released open-source to help developers build tools that better
reflect Google's robots.txt parsing and matching.

For webmasters, we included a runnable class `RobotsParserApp` which is a small
application that allows testing a single URL and several user-agents against a
robots.txt.

## Development

### Prerequisites

You need VampireApi to build this project.
You can install it over nuget (https://www.nuget.org/packages/VampireApi/):

### Build it

#### Using cli

Standard maven commands work here.

```
$ dotnet build
```

### Run it

#### Using cli

not yet supported

```
$ dotnet ./bin/Debug/netcoreapp3.1/RobotsTxt.dll
```


## Notes

Parsing of robots.txt files themselves is done exactly as in the production
version of Googlebot, including how percent codes and unicode characters in
patterns are handled. The user must ensure however that the URI passed to the
`Matcher` methods, or to the `--url` parameter of the application, follows the
format specified by RFC3986, since this library will not perform full
normalization of those URI parameters. Only if the URI is in this format, the
matching will be done according to the REP specification.

## License

The robots.txt parser and matcher Java and C# library is licensed under the terms of
the Apache license. See LICENSE for more information.

## Source Code Headers

Every file containing source code must include copyright and license
information. This includes any JS/CSS files that you might be serving out to
browsers. (This is to help well-intentioned people avoid accidental copying
that doesn't comply with the license.)

Apache header:

    Copyright 2020 Google LLC

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        https://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.

It can be done easily by using the
[addlicense](https://github.com/google/addlicense) tool.

Install it:

```
$ go get -u github.com/google/addlicense
```

Use it like this to make sure all files have the licence:

```
$ ~/go/bin/addlicense -c "Google LLC" -l apache .
```
