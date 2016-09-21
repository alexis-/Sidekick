﻿// 
// The MIT License (MIT)
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

namespace SQLite.Net.Platform.Test.Generic
{
  using System;
  using System.Runtime.InteropServices;

  using SQLite.Net.Interop;

  internal static class SQLiteApiGenericInternal
  {
    #region Methods

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_blob",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern byte[] ColumnBlob(IntPtr stmt, int index);

    public static byte[] ColumnByteArray(IntPtr stmt, int index)
    {
      int length = sqlite3_column_bytes(stmt, index);
      var result = new byte[length];
      if (length > 0)
        Marshal.Copy(sqlite3_column_blob(stmt, index), result, 0, length);
      return result;
    }

    public static string ColumnName16(IntPtr stmt, int index)
    {
      return Marshal.PtrToStringUni(sqlite3_column_name16(stmt, index));
    }

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_blob",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_bind_blob(
      IntPtr stmt, int index, byte[] val, int n, IntPtr free);

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_double",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_bind_double(IntPtr stmt, int index, double val);

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_bind_int(IntPtr stmt, int index, int val);

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int64",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_bind_int64(IntPtr stmt, int index, long val);

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_null",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_bind_null(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_parameter_index",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_bind_parameter_index(
      IntPtr stmt, [MarshalAs(UnmanagedType.LPStr)] string name);

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_text16",
       CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int sqlite3_bind_text16(
      IntPtr stmt, int index, [MarshalAs(UnmanagedType.LPWStr)] string val, int n, IntPtr free);

    [DllImport("sqlite3", EntryPoint = "sqlite3_busy_timeout",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_busy_timeout(IntPtr db, int milliseconds);

    [DllImport("sqlite3", EntryPoint = "sqlite3_changes",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_changes(IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_close",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_close(IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_initialize",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_initialize();

    [DllImport("sqlite3", EntryPoint = "sqlite3_shutdown",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_shutdown();

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_blob",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr sqlite3_column_blob(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_bytes",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_column_bytes(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_count",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_column_count(IntPtr stmt);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_double",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern double sqlite3_column_double(IntPtr stmt, int index);

    //        [DllImport("sqlite3", EntryPoint = "sqlite3_column_name", CallingConvention = CallingConvention.Cdecl)]
    //        private extern IntPtr ColumnNameInternal(IntPtr stmt, int index);

    //        [DllImport("sqlite3", EntryPoint = "sqlite3_column_name", CallingConvention = CallingConvention.Cdecl)]
    //        public string ColumnName(IntPtr stmt, int index)
    //        {
    //            return ColumnNameInternal(stmt, index);
    //        }

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_int",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_column_int(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_int64",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern long sqlite3_column_int64(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_text16",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr sqlite3_column_text16(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_type",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern ColType sqlite3_column_type(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_config",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_config(ConfigOption option);

    [DllImport("sqlite3", EntryPoint = "sqlite3_enable_load_extension",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_enable_load_extension(IntPtr db, int onoff);

    [DllImport("sqlite3", EntryPoint = "sqlite3_errmsg16",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr sqlite3_errmsg16(IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_finalize",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_finalize(IntPtr stmt);

    [DllImport("sqlite3", EntryPoint = "sqlite3_last_insert_rowid",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern long sqlite3_last_insert_rowid(IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_open",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_open(
      [MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_open(
      [MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db, int flags, IntPtr zvfs);

    [DllImport("sqlite3", EntryPoint = "sqlite3_open16",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_open16(
      [MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_open_v2(
      byte[] filename, out IntPtr db, int flags, IntPtr zvfs);

    [DllImport("sqlite3", EntryPoint = "sqlite3_prepare16_v2",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_prepare16_v2(
      IntPtr db, [MarshalAs(UnmanagedType.LPWStr)] string sql, int numBytes, out IntPtr stmt,
      IntPtr pzTail);

    [DllImport("sqlite3", EntryPoint = "sqlite3_reset",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_reset(IntPtr stmt);

    [DllImport("sqlite3", EntryPoint = "sqlite3_step",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_step(IntPtr stmt);

    [DllImport("sqlite3", EntryPoint = "sqlite3_win32_set_directory",
       CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int sqlite3_win32_set_directory(
      uint directoryType, string directoryPath);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_name16",
       CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr sqlite3_column_name16(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_extended_errcode",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern ExtendedResult sqlite3_extended_errcode(IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_libversion_number",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_libversion_number();

    [DllImport("sqlite3", EntryPoint = "sqlite3_sourceid",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr sqlite3_sourceid();

    #endregion



    #region Backup

    [DllImport("sqlite3", EntryPoint = "sqlite3_backup_init",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr sqlite3_backup_init(
      IntPtr destDB, [MarshalAs(UnmanagedType.LPStr)] string destName, IntPtr srcDB,
      [MarshalAs(UnmanagedType.LPStr)] string srcName);

    [DllImport("sqlite3", EntryPoint = "sqlite3_backup_step",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_backup_step(IntPtr backup, int pageCount);

    [DllImport("sqlite3", EntryPoint = "sqlite3_backup_finish",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern Result sqlite3_backup_finish(IntPtr backup);

    [DllImport("sqlite3", EntryPoint = "sqlite3_backup_remaining",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_backup_remaining(IntPtr backup);

    [DllImport("sqlite3", EntryPoint = "sqlite3_backup_pagecount",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_backup_pagecount(IntPtr backup);

    [DllImport("sqlite3", EntryPoint = "sqlite3_sleep",
       CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_sleep(int millis);

    #endregion
  }
}