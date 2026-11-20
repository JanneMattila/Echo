using System.Collections.Generic;

namespace Echo.Models;

public class McpInfoModel
{
    public string Endpoint { get; set; } = string.Empty;

    public List<McpToolInfo> Tools { get; set; } = [];
}

public class McpToolInfo
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
