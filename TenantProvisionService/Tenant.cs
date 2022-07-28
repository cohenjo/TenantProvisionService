using System;
namespace TenantProvisionService;
using k8s.Models;
using System.Text.Json.Serialization;

/// <summary>
/// Tenant Class exposed in the Rest API.
/// This is what we expect to get externally to be able to trigger the work with k8s
/// </summary>
public class Tenant 
{
	public string Id { get; set; } = null!;
	public string Cluster { get; set; } = null!;
	public string Db { get; set; } = null!;
	public string? Schema { get; set; }
	public string SourceName { get; set; } = null!;
	public string SourceNamespace { get; set; } = "default";
	public string Type { get; set; } = "sqlServer";

}
