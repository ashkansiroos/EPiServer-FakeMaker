Help features for test driving EPiServer CMS 7 and 8
========

This is the updated version of FakeMaker. I've added 2 features

1-Supporting EntryContentBase for Commerce system
You can now easily add your products to your fake. It is as easy as creating FakePages. For instance:

 var shopRoot = FakePage.Create("ShopRoot");
 var fakePhone = FakeEntryContent.Create<PhoneProduct>("Iphone 6").ChildOf(shopRoot);
_fake.AddToRepository(shopRoot);

2-Supporting ContentLoader
In our code, we used ContentLoader in many places while we also used ContentRepository. Of course I wanted to support the ContentLoader when I call AddToRepository :)
You can have access to ContentLoader the same as ContentRepository by FakeMaker.ContentLoader.
Just to mention, it make more sense to create your content in FakeMaker first and then use ContentLoader, ContentRepository, and serviceLocator that it creates for you.
