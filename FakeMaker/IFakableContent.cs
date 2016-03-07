using EPiServer;
using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EPiFakeMaker
{
   public  interface IFakableContent
    {
        ContentData Content { get;  }
        IList<IFakableContent> Children { get; }

        Expression<Func<IContentRepository, IContent>> RepoGet { get; }
        Expression<Func<IContentLoader, IContent>> ContentLoaderGet { get; }
    }
}
