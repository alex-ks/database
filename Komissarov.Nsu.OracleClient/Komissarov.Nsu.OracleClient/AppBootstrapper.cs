using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using Autofac;
using Komissarov.Nsu.OracleClient.Accessor;
using Komissarov.Nsu.OracleClient.ViewModels;

namespace Komissarov.Nsu.OracleClient
{
	class AppBootstrapper : AutofacBootstrapper<MainViewModel>
	{
		protected override void ConfigureContainer( ContainerBuilder builder )
		{
			base.ConfigureContainer( builder );
			builder.RegisterType<MainViewModel>( ).AsSelf( ).SingleInstance( );
			builder.RegisterType<MainViewModel>( ).As<IAccessProvider>( ).SingleInstance( );
		}
	}
}
