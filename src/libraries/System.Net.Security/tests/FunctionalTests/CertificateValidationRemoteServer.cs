// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Sockets;
using System.Net.Test.Common;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.DotNet.XUnitExtensions;
using Xunit;

namespace System.Net.Security.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    public class CertificateValidationRemoteServer
    {
        [Fact]
        [OuterLoop("Uses external servers")]
        public async Task CertificateValidationRemoteServer_EndToEnd_Ok()
        {
            if (PlatformDetection.IsWindows7)
            {
                // https://github.com/dotnet/corefx/issues/42339
                return;
            }

            using (var client = new TcpClient(AddressFamily.InterNetwork))
            {
                await client.ConnectAsync(Configuration.Security.TlsServer.IdnHost, Configuration.Security.TlsServer.Port);

                using (SslStream sslStream = new SslStream(client.GetStream(), false, RemoteHttpsCertValidation, null))
                {
                    await sslStream.AuthenticateAsClientAsync(Configuration.Security.TlsServer.IdnHost);
                }
            }
        }

        // MacOS has has special validation rules for apple.com and icloud.com
        [ConditionalTheory]
        [OuterLoop("Uses external servers")]
        [InlineData("www.apple.com")]
        [InlineData("www.icloud.com")]
        [PlatformSpecific(TestPlatforms.OSX)]
        public async Task CertificateValidationApple_EndToEnd_Ok(string host)
        {
            using (var client = new TcpClient())
            {
                try
                {
                    await client.ConnectAsync(host, 443);
                }
                catch (Exception ex)
                {
                    // if we cannot connect skip the test instead of failing.
                    new SkipTestException($"Unable to connect to '{host}': {ex.Message}");
                }


                using (SslStream sslStream = new SslStream(client.GetStream(), false, RemoteHttpsCertValidation, null))
                {
                    await sslStream.AuthenticateAsClientAsync(host);
                }
            }
        }

        private bool RemoteHttpsCertValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Assert.Equal(SslPolicyErrors.None, sslPolicyErrors);

            return true;
        }
    }
}
