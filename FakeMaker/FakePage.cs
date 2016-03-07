using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web;
using Moq;

namespace EPiFakeMaker
{
    public class FakePage : FakeContentBase<FakePage>
    {
        public virtual PageData Page { get { return Content as PageData; } }

        public static FakePage Create(string pageName)
        {
            return Create<PageData>(pageName);
        }

        public static FakePage Create<T>(string pageName) where T : PageData, new()
        {
            var fake = new FakePage { Content = new T() };

            fake.Page.Property["PageName"] = new PropertyString(pageName);

            fake.WithReferenceId(Randomizer.Next(10, 1000));

            fake.VisibleInMenu();

            fake.RepoGet = repo => repo.Get<T>(fake.Page.ContentLink);

            fake.ContentLoaderGet = loader => loader.Get<T>(fake.Page.ContentLink);

            return fake;
        }

        public virtual FakePage AsStartPage()
        {
            if (_siteDefinitonMock == null)
            {
                _siteDefinitonMock = Helpers.SetupSiteDefinition();
            }

            _siteDefinitonMock.SetupGet(def => def.StartPage).Returns(Page.ContentLink);

            return this;
        }

        public override FakePage WithReferenceId(int referenceId)
        {
            Content.Property["PageLink"] = new PropertyPageReference(new PageReference(referenceId));

            return this;
        }

        public virtual FakePage VisibleInMenu()
        {
            return SetMenuVisibility(true);
        }

        public virtual FakePage HiddenFromMenu()
        {
            return SetMenuVisibility(false);
        }

        public virtual FakePage SetMenuVisibility(bool isVisible)
        {
            return WithProperty("PageVisibleInMenu", new PropertyBoolean(isVisible));
        }

        public override FakePage WithLanguageBranch(string languageBranch)
        {
            return WithProperty("PageLanguageBranch", new PropertyString(languageBranch));
        }

        public override FakePage WithContentTypeId(int contentTypeId)
        {
            return WithProperty("PageTypeID", new PropertyNumber(contentTypeId));
        }

        public override FakePage StopPublishOn(DateTime stopPublishDate)
        {
            return WithProperty("PageStopPublish", new PropertyDate(stopPublishDate));
        }

        public override FakePage WorkStatus(VersionStatus status)
        {
            return WithProperty("PageWorkStatus", new PropertyNumber((int)status));
        }

        public override FakePage ChildOf(IFakableContent parent)
        {
            parent.Children.Add(this);

            return WithProperty("PageParentLink", new PropertyPageReference((parent.Content as IContent ).ContentLink));
        }

        protected override void SetStartPublishDate(DateTime publishDate)
        {
            throw new NotImplementedException();
        }
    }
}
