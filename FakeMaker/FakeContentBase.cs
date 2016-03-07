using EPiServer;
using EPiServer.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EPiServer.Web;
using System.Linq;


namespace EPiFakeMaker
{
    public abstract class FakeContentBase <T> : IFakableContent where T : IFakableContent
    {
        public virtual ContentData Content { get; protected set; }

        protected readonly IList<IFakableContent> _children;

        protected Mock<SiteDefinition> _siteDefinitonMock;

        protected static readonly Random Randomizer = new Random();


        public virtual IList<IFakableContent> Children { get { return _children; } }

        public Expression<Func<IContentRepository, IContent>> RepoGet { get; protected set; }
        public Expression<Func<IContentLoader, IContent>> ContentLoaderGet { get; protected set; }

        protected FakeContentBase()
        {
            _children = new List<IFakableContent>();
        }

        //public virtual T WithChildren(IEnumerable<IFakableContent> children)
        //{
        //    children.ToList().ForEach(c => c.ChildOf(this));

        //    return (T)(object)this;
        //}
        public virtual T To<T>() where T : ContentData
        {
            return Content as T;
        }

        public virtual T PublishedOn(DateTime publishDate)
        {
            PublishedOn(publishDate, null);

            return (T)(object)this;
        }

        public virtual T PublishedOn(DateTime publishDate, DateTime? stopPublishDate)
        {
            SetStartPublishDate(publishDate);

            WorkStatus(VersionStatus.Published);

            StopPublishOn(stopPublishDate.HasValue ? stopPublishDate.Value : publishDate.AddYears(1));

            return (T)(object)this;
        }

        public  T WithProperty(string propertyName, PropertyData propertyData)
        {
            Content.Property[propertyName] = propertyData;

            return (T)(object)this;
        }

        public abstract T WithReferenceId(int referenceId);

        public abstract T WithLanguageBranch(string languageBranch);

        public abstract T WithContentTypeId(int contentTypeId);

        public abstract T StopPublishOn(DateTime stopPublishDate);

        public abstract T WorkStatus(VersionStatus status);

        public abstract T ChildOf(IFakableContent parent);

        protected abstract void SetStartPublishDate(DateTime publishDate);
        
    }
}
