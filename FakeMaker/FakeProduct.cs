using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiFakeMaker
{
    public class FakeEntryContent : FakeContentBase<FakeEntryContent>
    {

        public virtual EntryContentBase Product { get { return Content as EntryContentBase; } }



        public static FakeEntryContent Create<T>(string productName) where T : EntryContentBase, new()
        {
            var fake = new FakeEntryContent { Content = (T)Activator.CreateInstance(typeof(T), new object[] { }) }; 

            fake.Product.Name = productName;
            fake.Product.DisplayName = productName;
            fake.Product.Code = productName;

            fake.WithReferenceId(Randomizer.Next(10, 1000));

            fake.RepoGet = repo => repo.Get<T>(fake.Product.ContentLink);
            fake.ContentLoaderGet = loader => loader.Get<T>(fake.Product.ContentLink);

            return fake;
        }

        public override FakeEntryContent ChildOf(IFakableContent parent)
        {
            parent.Children.Add(this);
            Product.ParentLink = (parent.Content as IContent).ContentLink;
            return this;
        }

        public override FakeEntryContent StopPublishOn(DateTime stopPublishDate)
        {
            Product.StopPublish = stopPublishDate;
            return this;
        }

        public override FakeEntryContent WithContentTypeId(int contentTypeId)
        {
            Product.ContentTypeID = contentTypeId;
            return this;
        }

        public override FakeEntryContent WithLanguageBranch(string languageBranch)
        {
            Product.Language = new CultureInfo(languageBranch);
            return this;
        }

        public override FakeEntryContent WithReferenceId(int referenceId)
        {
            Product.ContentLink = new ContentReference(referenceId);
            return this;
        }

        public override FakeEntryContent WorkStatus(VersionStatus status)
        {
            Product.Status = status;
            return this;
        }

        protected override void SetStartPublishDate(DateTime publishDate)
        {
            Product.StartPublish = publishDate;
        }
    }
}
