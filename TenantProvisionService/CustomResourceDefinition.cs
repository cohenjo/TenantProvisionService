using System;

using k8s;
using k8s.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "This is just an example.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "CS8618:TypeNamesShouldNotMatchNamespaces", Justification = "This is just an example.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "This is just an example.")]

namespace TenantProvisionService
{
	/// class representing CRD
	public class CustomResourceDefinition
	{
		/// API version
		public string Version { get; set; } = null!;

		/// api group
		public string Group { get; set; } = null!;

		/// CRD plural name
		public string PluralName { get; set; } = null!;

		/// CRD kind
		public string Kind { get; set; } = null!;

		/// CRD namespace
		public string Namespace { get; set; } = null!;
	}

	/// Class for a single Resource
	public abstract class CustomResource : IKubernetesObject
	{
		/// metadata
		[JsonProperty(PropertyName = "metadata")]
		public V1ObjectMeta Metadata { get; set; } = null!;
		public abstract string ApiVersion { get; set; }
		public abstract string Kind { get; set; }
	}

	/// CR adding Spec and status
	public abstract class CustomResource<TSpec, TStatus> : CustomResource
	{
		/// spec is what we want
		[JsonProperty(PropertyName = "spec")]
		public TSpec Spec { get; set; } = default(TSpec)!;

		/// status is what we have and monitor
		[JsonProperty(PropertyName = "CStatus")]
		public TStatus Status { get; set; } = default(TStatus)!;
	}

	/// a list of CRD
	public class CustomResourceList<T> : KubernetesObject
	where T : CustomResource
	{
		/// metadata
		public V1ListMeta Metadata { get; set; } = null!;
		/// CRD list items
		public List<T> Items { get; set; } = null!;
	}
}
