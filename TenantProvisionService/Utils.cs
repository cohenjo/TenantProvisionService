using System;
using k8s.Models;
using System.Collections.Generic;
namespace TenantProvisionService
{
	public class Utils
	{
		/// creats a CRD definition
		public static CustomResourceDefinition  MakeCRD()
		{
			var myCRD = new CustomResourceDefinition()
			{
				Kind = "ClusterExecuter",
				Group = "dbschema.microsoft.com",
				Version = "v1alpha1",
				PluralName = "clusterexecuters",
			};

			return myCRD;
		}


		/// creats a Cluster Executer instance
		public static ClusterExecuter MakeCLusterExecuter(Tenant tenant)
		{
			var myCResource = new ClusterExecuter()
			{
				Kind = "ClusterExecuter",
				ApiVersion = "dbschema.microsoft.com/v1alpha1",
				Metadata = new V1ObjectMeta
				{
					Name = tenant.Id,
					NamespaceProperty = tenant.SourceNamespace,
				},
				// spec
				Spec = new ClusterExecuterSpec
				{
					ClusterUri = tenant.Cluster,
					ApplyTo = new Filter(tenant.Cluster, tenant.Db, tenant.Schema),
					ConfigMapName = new NamespacedName(tenant.SourceNamespace, tenant.SourceName),
					Type = tenant.Type
				},
			};
			return myCResource;
		}
	}
}

