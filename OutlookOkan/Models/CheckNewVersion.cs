﻿using System;
using System.Net;
using System.Reflection;

namespace OutlookOkan.Models
{
    /// <summary>
    /// 新しいバージョンの有無を確認する。
    /// </summary>
    internal sealed class CheckNewVersion
    {
        /// <summary>
        /// 新バージョンのダウンロード可否を返す。
        /// </summary>
        /// <returns>新バージョンのダウンロード可否</returns>
        internal bool IsCanDownloadNewVersion()
        {
            using (var client = new WebClient())
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    client.Encoding = System.Text.Encoding.UTF8;
                    var versionFile = client.DownloadString("https://raw.githubusercontent.com/t-miyake/OutlookOkan/master/version");
                    if (string.IsNullOrEmpty(versionFile)) return false;

                    var fetchedVersion = int.Parse(versionFile.Replace(".", ""));
                    return fetchedVersion > GetCurrentVersion();

                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 現在使用しているアドインのバージョンを取得する。
        /// </summary>
        /// <returns>現在使用しているアドインのバージョン</returns>
        private int GetCurrentVersion()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            return int.Parse(assemblyName.Version.Major.ToString() + assemblyName.Version.Minor.ToString() + assemblyName.Version.Build.ToString() + assemblyName.Version.Revision.ToString());
        }
    }
}