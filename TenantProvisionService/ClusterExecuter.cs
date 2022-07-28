

using k8s;
using k8s.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TenantProvisionService
{
	/// Cluster Executer CRD
	public class ClusterExecuter : IKubernetesObject<V1ObjectMeta>
	{

		/// API version
		//public string Version { get; set; } = "v1alpha1";

		/// api group
		//public string Group { get; set; } = "dbschema.microsoft.com";

		/// CRD plural name
		//public string PluralName { get; set; } = "clusterexecuters";

		/// CRD kind
		[JsonPropertyName("kind")]
		public string Kind { get; set; } = "ClusterExecuter";

		/// CRD namespace
		//[JsonProperty(PropertyName = "namespace")]
		//public string Namespace { get; set; } = "default";

		[JsonPropertyName("apiVersion")]
		public string ApiVersion { get; set; }  = "dbschema.microsoft.com/v1alpha1"!;
		/// metadata
		[JsonPropertyName("metadata")]
		public V1ObjectMeta Metadata { get; set; } = null!;

		/// spec is what we want
		[JsonPropertyName("spec")]
		public ClusterExecuterSpec Spec { get; set; } = default(ClusterExecuterSpec)!;

		/// status is what we have and monitor
		[JsonPropertyName("status")]
		public ClusterExecuterStatus Status { get; set; } = default(ClusterExecuterStatus)!;

		/// Convert to String
		public override string ToString()
		{
			var labels = "{";
			// if (Metadata.Labels.Count > 0)
			// {
			// 	foreach (var kvp in Metadata.Labels)
			// 	{
			// 		labels += kvp.Key + " : " + kvp.Value + ", ";
			// 	}
			// }
			labels = labels.TrimEnd(',', ' ') + "}";

			return $"{Metadata.Name} (Labels: {labels}), Spec: {Spec.ClusterUri}, status: executed: {Status.Executed}, running: {Status.Running}";
		}
	}
	/// Cluster Executer CRD Cpec 
	public class ClusterExecuterSpec
	{
		/// The cluster URI
		[JsonPropertyName("clusterUri")]
		public string ClusterUri { get; set; } = null!;

		/// The Kusto cluster db to execute
		[JsonPropertyName("applyTo")]
		public Filter ApplyTo { get; set; } = null!;

		/// The schema configmap
		[JsonPropertyName("configMapName")]
		public NamespacedName ConfigMapName { get; set; } = null!;

		/// faile 
		[JsonPropertyName("failIfDataLoss")]
		public Boolean FailIfDataLoss { get; set; } = false!;

		[JsonPropertyName("type")]
		public string Type { get; set; } = "sqlServer";

		[JsonPropertyName("revision")]
		public int Revision { get; set; } = 0;

	}
	/// Cluster Executer CRD Status 
	public class ClusterExecuterStatus : V1Status
	{
		/// Did the executer execute
		[JsonPropertyName("executed")]
		public Boolean Executed { get; set; } = false!;
		/// Is the executer currently running
		[JsonPropertyName("running")]
		public Boolean Running { get; set; } = false!;
		/// Did the executer fail to execute		
		[JsonPropertyName("failed")]
		public Boolean Failed { get; set; } = false!;

		[JsonExtensionData]
		public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
	}

	/// namespaced name 
	public class NamespacedName
	{
		public NamespacedName(string @namespace, string name)
		{
			Namespace = @namespace;
			Name = name;
		}

		/// The namespace
		[JsonPropertyName("namespace")]
		public string Namespace { get; set; } = null!;

		/// The name
		[JsonPropertyName("name")]
		public string Name { get; set; } = null!;

	}

	public class Filter
	{

		public Filter(string clusterUri, string db, string? schema)
		{
			ClusterUris = new string[] { clusterUri };
			Db = db;
			Schema = schema;
		}

		[JsonConstructor]
		public Filter(string[] clusterUris, string db, string[] dBS, string webhook, string? schema, bool create, bool regexp)
		{
			ClusterUris = clusterUris;
			Db = db;
			DBS = dBS;
			Webhook = webhook;
			if (schema is not null)
			{
				Schema = schema;
			}
			Create = create;
			Regexp = regexp;
		}

		/// The cluster URI
		[JsonPropertyName("clusterUris")]
		public string[] ClusterUris { get; set; } = null!;

		/// The DB
		[JsonPropertyName("db")]
		public string Db { get; set; } = null!;

		[JsonPropertyName("dbs")]
		public string[] DBS { get; set; } = null!;

		[JsonPropertyName("webhook")]
		public string Webhook { get; set; } = null!;

		/// The schema
		[JsonPropertyName("schema")]
		public string? Schema { get; set; } 

		/// The schema
		[JsonPropertyName("create")]
		public bool Create { get; set; } = true;

		[JsonPropertyName("regexp")]
		public bool Regexp { get; set; } = true;
	}

	public class ClusterExecuterList : IKubernetesObject<V1ListMeta>
	{

		[JsonPropertyName("kind")]
		public string Kind { get; set; } = "ClusterExecuter";
		
		[JsonPropertyName("apiVersion")]
		public string ApiVersion { get; set; } = "dbschema.microsoft.com/v1alpha1"!;

		/// metadata
		public V1ListMeta Metadata { get; set; } = null!;
		/// CRD list items
		public List<ClusterExecuter> Items { get; set; } = null!;
	}
}