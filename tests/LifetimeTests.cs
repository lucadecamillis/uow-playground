using Unity;
using Unity.Lifetime;
using uow.playground.Context;
using uow.playground.Contract.Interfaces;
using uow.playground.Repositories;
using uow.playground.Services;
using Xunit;

namespace uow.playground.tests
{
	public class LifetimeTests
	{
		[Fact]
		public void Lifetime_AssureDisposed()
		{
			IUnityContainer container = WireUp();

			DbContext dbContext = container.Resolve<DbContext>();

			Assert.False(dbContext.Disposed);

			IEntityService entityService = container.Resolve<IEntityService>();

			container.Dispose();

			Assert.True(dbContext.Disposed);
		}

		[Fact]
		public void Lifetime_Hierarchical()
		{
			IUnityContainer container = WireUp();

			DbContext mainDbContext = container.Resolve<DbContext>();

			IUnityContainer childContainer = container.CreateChildContainer();
			{
				DbContext childDbContext = childContainer.Resolve<DbContext>();

				Assert.False(childDbContext.Disposed);

				IEntityService entityService = container.Resolve<IEntityService>();

				childContainer.Dispose();

				Assert.True(childDbContext.Disposed);
			}

			Assert.False(mainDbContext.Disposed);
		}

		private IUnityContainer WireUp()
		{
			IUnityContainer container = new UnityContainer();

			container.RegisterType<DbContext>(new HierarchicalLifetimeManager());
			container.RegisterType<IUnityOfWork, UnityOfWork>();
			container.RegisterType<IEntityRepository, EntityRepository>();
			container.RegisterType<IDependencyRepository, DependencyRepository>();
			container.RegisterType<IEntityService, EntityService>();

			return container;
		}
	}
}