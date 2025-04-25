using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DefaultNamespace
{

  public class GitHubReleaseUpdateDto
  {
    [JsonPropertyName("body")]
    public string Body { get; set; }
  }

  public class GitHubRestApiHelper
  {
    const string c_gitHubApiBaseUrl = "https://api.github.com/repos/{0}/{1}/{2}";

    public string RepositoryOwnerName { get; }
    public string RepositoryName { get; }

    public GitHubRestApiHelper(string repositoryOwnerName, string repositoryName)
    {
      ArgumentException.ThrowIfNullOrEmpty(repositoryOwnerName);
      ArgumentException.ThrowIfNullOrEmpty(repositoryName);

      RepositoryOwnerName = repositoryOwnerName;
      RepositoryName = repositoryName;
    }

    public JsonDocument GetReleaseByTag(string tag)
    {
      var url = GetApiUrl("releases/tags/" + tag);
      var response = ExecuteRequest(url, HttpMethod.Get);

      using (var resultStream = response.Content.ReadAsStream())
      {
        var jsonDocument = JsonDocument.Parse(resultStream);
        return jsonDocument;
      }
    }

    public bool UpdateReleaseBodyByID(int id, string body, string gitHubToken)
    {
      var url = GetApiUrl("releases/" + id);
      var jsonContent = JsonContent.Create(new GitHubReleaseUpdateDto { Body = body });
      var result = ExecuteRequest(url, HttpMethod.Patch, jsonContent, gitHubToken);
      return result.IsSuccessStatusCode;
    }

    private string GetApiUrl(string endpoint)
    {
      return string.Format(c_gitHubApiBaseUrl, RepositoryOwnerName, RepositoryName, endpoint);
    }

    private HttpResponseMessage ExecuteRequest(string url, HttpMethod verb, HttpContent content = null, string gitHubToken = null)
    {
      using (var client = new HttpClient())
      using (var message = new HttpRequestMessage(verb, url))
      {
        if (gitHubToken != null)
          message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", gitHubToken);

        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        message.Headers.Add("User-Agent", "bonsai");

        message.Content = content;
        var response = client.Send(message);
        return response;
      }
    }
  }
}