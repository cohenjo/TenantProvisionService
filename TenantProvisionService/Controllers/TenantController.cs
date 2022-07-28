using System;
using k8s;
using k8s.Autorest;
using k8s.Models;
using k8s.Util.Common;
using k8s.Util.Common.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace TenantProvisionService.Controllers;

[ApiController]
[Route("[controller]")]
public class TenantController : ControllerBase
{
	private IKubernetes client;
	private GenericKubernetesApi generic;

	private readonly ILogger<TenantController> _logger;

	public TenantController(ILogger<TenantController> logger)
	{
		_logger = logger;
		// Load from the default kubeconfig on the machine.
		var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

		// Use the config object to create a client.
		client = new Kubernetes(config);
		// creating a K8s client for the CRD
		var myCRD = Utils.MakeCRD();
		Console.WriteLine("working with CRD: {0}.{1}", myCRD.PluralName, myCRD.Group);
		generic = new GenericKubernetesApi(
				apiGroup: myCRD.Group,
				apiVersion: myCRD.Version,
				resourcePlural: myCRD.PluralName,
				apiClient: client);
	}

	//[HttpGet(Name = "GetTenant")]
	//public IEnumerable<ClusterExecuter> Get()
	//{
	//	// listing the cr instances
	//	Console.WriteLine("CR list:");
	//	var cts = new CancellationTokenSource();
	//	var aListOfPods = ListPodsInNamespace(Namespaces.NamespaceDefault, cts.Token);
	//	foreach (var cr in aListOfPods.Items)
	//	{
	//		Console.WriteLine("- CR Item {0} = {1}", aListOfPods.Items.IndexOf(cr), cr.Metadata.Name);
	//	}

	//	return aListOfPods.Items;
	//}

	[HttpGet(Name = "GetExecuterStatus")]
	public ClusterExecuter GetExecuterStatus(string executer)
	{
		// listing the cr instances
		Console.WriteLine("CR list:");
		var cts = new CancellationTokenSource();
		var ClusterExecuter = GetNamespacedClusterExecuter(Namespaces.NamespaceDefault, executer, cts.Token);
		_logger.LogInformation("- CR Item {0} = {1}", ClusterExecuter.Metadata.Name, ClusterExecuter.Status.ToString());
		
		return ClusterExecuter;
	}

	[HttpPost(Name = "PostNewTenant")]
	public ClusterExecuter Post(Tenant newTenant)
	{
		var cts = new CancellationTokenSource();
		_logger.LogInformation("Adding new tenant {0}", newTenant);
		var ce = Utils.MakeCLusterExecuter(newTenant);
		_logger.LogInformation("generating {0}", ce);

		JsonSerializerOptions options = new()
		{
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		string jsonString =
			JsonSerializer.Serialize<ClusterExecuter>(ce, options);

		_logger.LogInformation("generating this: {0}", jsonString);

		var nce = CreateClusterExecuter(ce, cts.Token);
		_logger.LogInformation("created {0}", nce);

		bool completed = false;
		while (!completed)
		{
			var gce = GetNamespacedClusterExecuter(nce.Metadata.NamespaceProperty, nce.Metadata.Name, cts.Token);
			if (gce is not null) {
				if (gce.Status is not null)
				{
					if (gce.Status.Executed)
					{
						completed = true;
						_logger.LogInformation("tenant created");
					}
					else
					{
						_logger.LogWarning("gce not yet executed");
						// sleep a min before checking again
						Thread.Sleep(1000 * 60);
					}
				}
				else
				{
					_logger.LogWarning("gce.status is null");
					// sleep a half min before checking again
					Thread.Sleep(1000 * 30);
				}
			}
			else
			{
				_logger.LogWarning("gce is null");
				// sleep a half min before checking again
				Thread.Sleep(1000 * 30);
			}
		}
		_logger.LogInformation("deleting the item post execution");

		DeleteClusterExecuter(nce, cts.Token);
		return nce;
	}

	private ClusterExecuter GetNamespacedClusterExecuter(string @namespace, string executer, CancellationToken cancellationToken)
	{
		var resp = Task.Run(
			async () => await generic.GetAsync<ClusterExecuter>(@namespace, executer, cancellationToken).ConfigureAwait(false), cancellationToken);

		return resp.Result;
	}

	private ClusterExecuterList ListPodsInNamespace(string @namespace, CancellationToken cancellationToken)
	{
		var resp = Task.Run(
			async () => await generic.ListAsync<ClusterExecuterList>(@namespace, cancellationToken).ConfigureAwait(false), cancellationToken);

		return resp.Result;
	}

	private ClusterExecuter CreateClusterExecuter(ClusterExecuter executer, CancellationToken cancellationToken)
	{
		try
		{
			var resp = Task.Run(
			async () => await generic.CreateAsync(executer, cancellationToken).ConfigureAwait(false), cancellationToken);

			return resp.Result;
		}
		catch (Exception e)
		{
			_logger.LogError("failed to create the object, reason: {0}\n {1}", e.Message, e.ToString());
			return executer;
		}
		
	}
	private bool DeleteClusterExecuter(ClusterExecuter executer, CancellationToken cancellationToken)
	{
		try
		{
			var resp = Task.Run(
			async () => await generic.DeleteAsync< ClusterExecuter>(executer.Metadata.NamespaceProperty,executer.Metadata.Name, cancellationToken).ConfigureAwait(false), cancellationToken);

			return resp.IsCompleted;
		}
		catch (Exception e)
		{
			_logger.LogError("Deletion reaised an issue, reason: {0}\n {1}", e.Message, e.ToString());
			return false;
		}

	}
}


