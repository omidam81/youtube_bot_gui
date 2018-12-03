using System;
using System.Diagnostics;
using NUnit.Framework;
using youtube_bot_lib.api;

namespace youtube_bot_test.api
{
    public class WebAdapterTest
    {
        [Test]
        public void getProxyFromXmlTest()
        {
            try
            {
                string proxy = WebAdapter.getProxyFromXml();
                Assert.IsNotNull(proxy);
                Assert.IsTrue(proxy.Length > 0);
                Debug.WriteLine(proxy);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }
    }
}