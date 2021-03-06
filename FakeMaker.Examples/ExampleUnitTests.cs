﻿using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using NUnit.Framework;

namespace EPiFakeMaker.Examples
{
	[TestFixture]
	public class ExampleUnitTests
	{
		private FakeMaker _fake;

		[SetUp]
		public void Setup()
		{
			_fake = new FakeMaker();
		}

		[Test]
		public void Get_descendants()
		{
			// Arrange
			var root = FakePage
				.Create("Root");

			var start = FakePage
				.Create("Start")
				.ChildOf(root);

			FakePage
				.Create("About us")
				.ChildOf(start);

			_fake.AddToRepository(root);

			// Act
			var descendants = ExampleFindPagesHelper.GetDescendantsOf(root.Page.ContentLink, _fake.ContentRepository);

			//Assert
			Assert.That(descendants.Count(), Is.EqualTo(2));
		}

		[Test]
		public void Get_descendants_by_using_ServiceLocator()
		{
			// Arrange
			var root = FakePage
				.Create("Root");

			var start = FakePage
				.Create("Start")
				.ChildOf(root);

			FakePage
				.Create("About us")
				.ChildOf(start);

			_fake.AddToRepository(root);

			// Act
			var descendants = ExampleFindPagesHelper.GetDescendantsOf(root.Page.ContentLink);

			//Assert
			Assert.That(descendants.Count(), Is.EqualTo(2));
		}

		[Test]
		public void Get_children_of_first_child()
		{
			// Arrange
			var root = FakePage
				.Create("Root");

			FakePage
				.Create("my page")
				.ChildOf(root);
			
			var start = FakePage
				.Create("Start")
				.ChildOf(root);

			FakePage
				.Create("About us")
				.ChildOf(start);

			FakePage
				.Create("Our services")
				.ChildOf(start);

			_fake.AddToRepository(root);

			// Act
			var children = ExampleFindPagesHelper.GetDescendantsOf(start.Page.ContentLink, _fake.ContentRepository);

			//Assert
			Assert.That(children.Count(), Is.EqualTo(2));
		}

		[Test]
		public void Get_published_only_pages()
		{
			// Arrange
			var lastWeek = DateTime.Today.AddDays(-7);
			var yesterday = DateTime.Today.AddDays(-1);

			var root = FakePage
				.Create("Root");

			var start = FakePage
				.Create("Start")
				.ChildOf(root)
				.PublishedOn(lastWeek);

			FakePage
				.Create("About us")
				.ChildOf(start)
				.PublishedOn(lastWeek, yesterday);

			FakePage
				.Create("Our services")
				.ChildOf(start)
				.PublishedOn(lastWeek);

			_fake.AddToRepository(root);

			// Act
			var pages = ExampleFindPagesHelper.GetAllPublishedPages(root.Page.ContentLink, _fake.ContentRepository);

			//Assert
			Assert.That(pages.Count(), Is.EqualTo(2));
		}

		[Test]
		public void Get_pages_visible_in_menu()
		{
			// Arrange
			var root = FakePage.Create("root");

			FakePage.Create("AboutUs").ChildOf(root).VisibleInMenu();
			FakePage.Create("OtherPage").ChildOf(root).HiddenFromMenu();
			FakePage.Create("Contact").ChildOf(root).VisibleInMenu();

			_fake.AddToRepository(root);

			// Act
			var pages = ExampleFindPagesHelper.GetMenu(root.Page.ContentLink, _fake.ContentRepository);

			// Assert
			Assert.That(pages.Count(), Is.EqualTo(2));
		}

		[Test]
		public void Get_pages_of_certain_pagedata_type()
		{
			// Arrange
			var root = FakePage.Create("root");

			FakePage.Create("AboutUs").ChildOf(root);
			FakePage.Create<CustomPageData>("OtherPage").ChildOf(root);
			FakePage.Create("Contact").ChildOf(root);

			_fake.AddToRepository(root);

			// Act
			var pages = ExampleFindPagesHelper.GetDescendantsOf<CustomPageData>(root.Page.ContentLink, _fake.ContentRepository);

			// Assert
			Assert.That(pages.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Get_pages_with_certain_pagetypeid()
		{
			// Arrange
			var root = FakePage.Create("root");

			FakePage.Create("AboutUs").ChildOf(root).WithContentTypeId(1);
			FakePage.Create("OtherPage").ChildOf(root).WithContentTypeId(2);
			FakePage.Create("Contact").ChildOf(root).WithContentTypeId(3);

			_fake.AddToRepository(root);

			// Act
			var pages = ExampleFindPagesHelper.GetChildrenOf(root.Page.ContentLink, _fake.ContentRepository).Where(p => p.ContentTypeID == 2);

			// Assert
			Assert.That(pages.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Get_pages_with_custom_property()
		{
			// Arrange
			var root = FakePage.Create("root");

			FakePage.Create("AboutUs").ChildOf(root);
			FakePage.Create("OtherPage").ChildOf(root).WithProperty("CustomProperty", new PropertyString("Custom value"));
			FakePage.Create("Contact").ChildOf(root);

			_fake.AddToRepository(root);

			// Act
			var pages =
				ExampleFindPagesHelper.GetChildrenOf(root.Page.ContentLink, _fake.ContentRepository)
					.Where(content => content.Property["CustomProperty"] != null && content.Property["CustomProperty"].Value.ToString() == "Custom value");

			// Assert
			Assert.That(pages.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Get_pages_with_certain_languagebranch()
		{
			// Arrange
			var root = FakePage.Create("root").WithLanguageBranch("en");

			FakePage.Create("AboutUs").ChildOf(root).WithLanguageBranch("en");
			FakePage.Create("OtherPage").ChildOf(root).WithLanguageBranch("sv");
			FakePage.Create("Contact").ChildOf(root).WithLanguageBranch("en");

			_fake.AddToRepository(root);

			// Act
			var pages =
				ExampleFindPagesHelper.GetChildrenOf(root.Page.ContentLink, _fake.ContentRepository)
					.Where(content => content is PageData && ((PageData)content).LanguageBranch == "sv");

			// Assert
			Assert.That(pages.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Get_descendants_from_with_children()
		{
			// Arrange
			var root =
				FakePage.Create("root")
					.WithChildren(
						new List<FakePage>
							{
								FakePage.Create("AboutUs"),
								FakePage.Create("News").WithChildren(new List<FakePage>
										{
											FakePage.Create("News item 1"), 
											FakePage.Create("News item 2")
										}),
								FakePage.Create("Contact")
							});


			_fake.AddToRepository(root);

			// Act
			var pages =
				ExampleFindPagesHelper.GetDescendantsOf(root.Page.ContentLink, _fake.ContentRepository);

			// Assert
			Assert.That(pages.Count(), Is.EqualTo(5));
		}

		[Test]
		public void Set_a_page_as_start_page()
		{
			var root = FakePage.Create<PageData>("Root");
			var start = FakePage.Create<PageData>("Start").ChildOf(root).AsStartPage();
			FakePage.Create<PageData>("Child").ChildOf(start);

			_fake.AddToRepository(root);

			Assert.That(ContentReference.StartPage, Is.EqualTo(start.Page.ContentLink));
		}

		[Test]
		public void Get_page_of_explicit_page_type()
		{
			// Arrange
			var customPage = FakePage
				.Create<CustomPageData>("MyCustomPage");

			_fake.AddToRepository(customPage);

			// Act
			var result = _fake.ContentRepository.Get<CustomPageData>(customPage.Page.ContentLink);

			// Assert
			Assert.IsNotNull(result);
		}

		[Test]
		public void Get_page_of_explicit_base_page_type()
		{
			// Arrange
			var fakePage = FakePage
				.Create<InteritsCustomPageData>("MyInheritedCustomPage");

			_fake.AddToRepository(fakePage);

			// Custom mocking that is not handled by FakeMaker
			_fake.GetMockForFakeContentRepository()
				.Setup(repo => repo.Get<CustomPageData>(fakePage.Page.ContentLink))
				.Returns(fakePage.To<CustomPageData>());

			// Act
			var result = _fake.ContentRepository.Get<CustomPageData>(fakePage.Page.ContentLink);

			// Assert
			Assert.IsNotNull(result);
			Assert.That(result is InteritsCustomPageData);
		}

		[Test]
		public void Get_children_as_explicit_page_type()
		{
			// Arrange
			var root = FakePage
				.Create("Root");

			var start = FakePage
				.Create("Start")
				.ChildOf(root).AsStartPage();

			var aboutUs = FakePage
				.Create<CustomPageData>("About us")
				.ChildOf(start);

			_fake.AddToRepository(root);

			var customPageDataList = new List<CustomPageData> { aboutUs.To<CustomPageData>() };

			// Custom mocking that is not handled by FakeMaker
			_fake.GetMockForFakeContentRepository()
				.Setup(repo => repo.GetChildren<CustomPageData>(ContentReference.StartPage))
				.Returns(customPageDataList);

			// Act
			var children = _fake.ContentRepository.GetChildren<CustomPageData>(ContentReference.StartPage);

			// Assert
			Assert.That(children.Count(), Is.EqualTo(1));

		}
	}

	public class CustomPageData : PageData
	{
		public string CustomPageName { get; set; }
	}

	public class InteritsCustomPageData : CustomPageData
	{
	}

	/// <summary>
	/// This is an example of a helper class.
	/// The repository is injected to the class.
	/// </summary>
	public static class ExampleFindPagesHelper
	{
		public static IEnumerable<IContent> GetChildrenOf(ContentReference root, IContentRepository repository)
		{
			return repository.GetChildren<IContent>(root);
		}

		public static IEnumerable<ContentReference> GetDescendantsOf(ContentReference root, IContentRepository repository)
		{
			return repository.GetDescendents(root);
		}

		public static IEnumerable<ContentReference> GetDescendantsOf(ContentReference root)
		{
			var repository = ServiceLocator.Current.GetInstance<IContentRepository>();

			return repository.GetDescendents(root);
		}

		public static IEnumerable<IContent> GetDescendantsOf<T>(ContentReference root, IContentRepository repository)
			where T : PageData
		{
			var descendants = GetDescendantsOf(root, repository);
			var pages = descendants
				.Select(repository.Get<IContent>)
				.OfType<T>();

			return pages;
		}

		public static IEnumerable<IContent> GetAllPublishedPages(ContentReference root, IContentRepository repository)
		{
			var descendants = GetDescendantsOf(root, repository);

			var references = descendants
				.Where(item => ToPage(item, repository).CheckPublishedStatus(PagePublishedStatus.Published));

			return references.Select(reference => ToPage(reference, repository)).Cast<IContent>().ToList();
		}

		private static PageData ToPage(ContentReference reference, IContentLoader repository)
		{
			var page = repository.Get<PageData>(reference);

			return page;
		}

		public static IEnumerable<IContent> GetMenu(ContentReference reference, IContentRepository repository)
		{
			var children = repository.GetChildren<PageData>(reference);

			return children.Where(page => page.VisibleInMenu).ToList();
		}
	}
}
