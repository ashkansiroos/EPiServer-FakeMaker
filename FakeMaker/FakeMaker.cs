using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.BaseLibrary;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Moq;

namespace EPiFakeMaker
{
    public class FakeMaker
    {
        private readonly Mock<IContentRepository> _contentRepo;

        public IContentRepository ContentRepository { get { return _contentRepo.Object; } }

        public FakeMaker(bool prepareServiceLocatorWithFakeRepository = true)
        {
            SetupMocksForClassFactory();

            _contentRepo = new Mock<IContentRepository>();

            if (!prepareServiceLocatorWithFakeRepository)
            {
                return;
            }

            PrepareServiceLocatorWith(_contentRepo.Object);
        }

        public Mock<IContentRepository> GetMockForFakeContentRepository()
        {
            return _contentRepo;
        }

        public void AddToRepository(FakePage fake)
        {
            _contentRepo
                .Setup(repo => repo.Get<IContent>(fake.Page.ContentLink))
                .Returns(fake.Page);

            _contentRepo
                .Setup(repo => repo.Get<PageData>(fake.Page.ContentLink))
                .Returns(fake.Page);

            _contentRepo
                .Setup(fake.RepoGet)
                .Returns(fake.Page);

            AddToRepository(fake.Children, fake);
        }

        private static void SetupMocksForClassFactory()
        {
            var fakeEpiBaseLibraryContext = new Mock<IContext>();
            fakeEpiBaseLibraryContext
                .Setup(fake => fake.RequestTime)
                .Returns(DateTime.Now);

            var fakeBaseFactory = new Mock<IBaseLibraryFactory>();
            fakeBaseFactory
                .Setup(factory => factory.CreateContext())
                .Returns(fakeEpiBaseLibraryContext.Object);

            ClassFactory.Instance = fakeBaseFactory.Object;
        }

        private static void PrepareServiceLocatorWith<T>(T repository)
        {
            var serviceLocator = new Mock<IServiceLocator>();

            serviceLocator
                .Setup(locator => locator.GetInstance<T>())
                .Returns(repository);

            ServiceLocator.SetLocator(serviceLocator.Object);
        }

        private void AddToRepository(IList<IFakableContent> fakeList, IFakableContent parent)
        {
            var contentList = fakeList.Select(fake => fake.Content as IContent).ToList();
            var pageDataList = fakeList.Where(fake => (fake.Content as PageData) != null)
                .Select(fake => fake.Content as PageData).ToList();
            var parentContentLink = (parent.Content as IContent).ContentLink;

            _contentRepo
                .Setup(repo => repo.GetChildren<IContent>(parentContentLink))
                .Returns(contentList);

            _contentRepo
                .Setup(repo => repo.GetChildren<PageData>(parentContentLink))
                .Returns(pageDataList);

            var parentDescendants = GetDescendantsOf(parent, new List<IContent>());

            _contentRepo
                .Setup(repo => repo.GetDescendents(parentContentLink))
                .Returns(parentDescendants);

            foreach (var fake in fakeList)
            {
                var item = fake;

                _contentRepo
                    .Setup(repo => repo.Get<IContent>((item.Content as IContent).ContentLink))
                    .Returns(item.Content as IContent);

                var fakePageItem = item as FakePage;
                if (fakePageItem != null)
                {
                    
                    _contentRepo
                        .Setup(repo => repo.Get<PageData>(fakePageItem.Page.ContentLink))
                        .Returns(fakePageItem.Page);
                }
                _contentRepo
                    .Setup(item.RepoGet)
                    .Returns(item.Content as IContent);

                var pageDescendants = GetDescendantsOf(item, new List<IContent>());

                _contentRepo
                    .Setup(repo => repo.GetDescendents((item.Content as IContent).ContentLink))
                    .Returns(pageDescendants);

                AddToRepository(item.Children, item);
            }
        }

        private static IEnumerable<ContentReference> GetDescendantsOf(IFakableContent fake, ICollection<IContent> descendants)
        {
            foreach (var child in fake.Children)
            {
                descendants.Add(child.Content as IContent);

                GetDescendantsOf(child, descendants);
            }

            return descendants.Select(descendant => descendant.ContentLink).ToList();
        }
    }
}
