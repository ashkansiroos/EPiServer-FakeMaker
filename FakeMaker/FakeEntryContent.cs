using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiFakeMaker
{
    public class FakeEntryContent : FakeContentBase
    {
        public override FakeContentBase WithReferenceId(int referenceId)
        {
            Content.Property["PageLink"] = new PropertyContentReference(new ContentReference(referenceId));

            return this;
        }
    }
}
