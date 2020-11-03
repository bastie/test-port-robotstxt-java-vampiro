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
/** Used in parsing process. */
public class ParseException : java.lang.Exception {
  public ParseException() :
    base(){
  }

  public ParseException(String message, java.lang.Throwable cause) :
    base(message, cause){
  }
}
}