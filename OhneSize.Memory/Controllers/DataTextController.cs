namespace OhneSize.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class DataTextController : ControllerBase {
    private readonly DataTextRepository _Repository;

    public DataTextController(
        DataTextRepository repository
        ) {
        this._Repository = repository;
    }

    [HttpGet("", Name = nameof(DataTextGetIndex))]
    public ActionResult<DataItem> DataTextGetIndex() {
        return this.Ok(
            this._Repository.GetAllDataItem(null));
    }

    [HttpGet("{name}", Name = nameof(DataTextGetByName))]
    public ActionResult DataTextGetByName(string name) {
        if (this._Repository.TryGetByName(name, out var item, out var _)) {
            return this.Ok(item);
        } else {
            return this.NotFound();
        }
    }

    [HttpPost("{name}", Name = nameof(DataTextPostByName))]
    public ActionResult DataTextPostByName(string name, [FromBody] string data) {
        var contentType = this.Request.Headers.ContentType.FirstOrDefault() ?? string.Empty;
        this._Repository.Upsert(name, data, contentType);
        return this.Ok();
    }

    [HttpPut("{name}", Name = nameof(DataTextPutByName))]
    public async Task<ActionResult> DataTextPutByName(string name) {
        var contentType = this.Request.Headers.ContentType.FirstOrDefault() ?? string.Empty;
        var sb = Brimborium.Text.StringBuilderPool.GetStringBuilder();
        using var streamReader = new StreamReader(this.Request.Body, Encoding.UTF8);
        var data = await streamReader.ReadToEndAsync();
        var upsert = this._Repository.Upsert(name, data, contentType);
        if (upsert.dataItem is null) {
            return this.Conflict();
        }
        return this.CreatedAtRoute(nameof(DataTextGetByName), new { name = name }, upsert.dataItem.AsDataItem());
    }

    [HttpDelete("{name}", Name = nameof(DataTextDeleteByName))]
    public ActionResult DataTextDeleteByName(string name) {
        if (name == "*") {
            this._Repository.Clear();
            return this.Ok();
        } else {
            if (this._Repository.Delete(name)) {
                return this.Ok();
            } else {
                return this.NotFound();
            }
        }
    }
}
