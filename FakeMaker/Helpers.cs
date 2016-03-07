using EPiServer.Web;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiFakeMaker
{
    class Helpers
    {
        public static Mock<SiteDefinition> SetupSiteDefinition()
        {
            var mock = new Mock<SiteDefinition>();

            mock.SetupGet(def => def.Name).Returns("FakeMakerSiteDefinition");

            SiteDefinition.Current = mock.Object;

            return mock;
        }
    }
}
