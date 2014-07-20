using BikeShare.Interfaces;
using BikeShare.Repositories;
using Ninject;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace BikeShare.Code
{
    /// <summary>
    /// Handles resolving dependencies and includes logic specific to the interfaces used by controllers.
    /// </summary>
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<Object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind<IAdminRepository>().To<AdminDbRepository>();
            kernel.Bind<IExploreRepository>().To<ExploreDbRepository>();
            kernel.Bind<IMaintenanceRepository>().To<MaintenanceDbRepository>();
            kernel.Bind<IWorkshopRepository>().To<WorkshopDbRepository>();
            kernel.Bind<IUserRepository>().To<UserDbRepository>();
            kernel.Bind<ICheckOutRepository>().To<CheckoutDbRepository>();
            kernel.Bind<IFinanceRepository>().To<FinanceDbRepository>();
            kernel.Bind<ISettingRepository>().To<SettingsDbRepository>();
        }
    }

}