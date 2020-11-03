/*
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at 
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *  
 *  Copyright © 2020 Sebastian Ritter
 */
using System;
using java = biz.ritter.javapi;

namespace com.google.commons.io {

    internal class ByteStreams {
        static internal byte [] toByteArray (java.io.InputStream inJ) {
            java.io.ByteArrayOutputStream baos = new java.io.ByteArrayOutputStream();
            byte[] buffer = new byte[32 * 1024];

            int bytesRead;
            while ((bytesRead = inJ.read(buffer)) > 0) {
                baos.write(buffer, 0, bytesRead);
            }
            byte[] bytes = baos.toByteArray();    
            return bytes;        
        }
    }

    
}