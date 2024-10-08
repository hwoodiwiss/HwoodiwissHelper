﻿using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using HwoodiwissHelper.Features.GitHub.Events;

namespace HwoodiwissHelper;

[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(JsonObject))]
[JsonSerializable(typeof(KeyValuePair<string, string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(WorkflowRun))]
[JsonSerializable(typeof(PullRequest))]
public partial class ApplicationJsonContext : JsonSerializerContext;

