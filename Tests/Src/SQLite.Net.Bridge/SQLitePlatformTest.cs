// 
// The MIT License (MIT)
// Copyright (c) 2016 Incogito
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#if __WIN32__
using _SQLitePlatformTest = SQLite.Net.Platform.Win32.SQLitePlatformWin32;

#elif WINDOWS_PHONE
using _SQLitePlatformTest = SQLite.Net.Platform.WindowsPhone8.SQLitePlatformWP8;
#elif __WINRT__
using _SQLitePlatformTest = SQLite.Net.Platform.WinRT.SQLitePlatformWinRT;
#elif __IOS__
using _SQLitePlatformTest = SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS;
#elif __ANDROID__
using _SQLitePlatformTest = SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid;
#elif __OSX__
using _SQLitePlatformTest = SQLite.Net.Platform.OSX.SQLitePlatformOSX;
#else
using _SQLitePlatformTest = SQLite.Net.Platform.Generic.SQLitePlatformGeneric;
#endif

// ReSharper disable once CheckNamespace

namespace SQLite.Net.Bridge.Tests
{
  public class SQLitePlatformTest : _SQLitePlatformTest
  {
  }
}