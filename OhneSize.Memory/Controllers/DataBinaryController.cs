namespace OhneSize.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]

public class DataBinaryController : ControllerBase {
    private readonly DataBinaryRepository _Repository;

    public DataBinaryController(
        DataBinaryRepository repository
        ) {
        this._Repository = repository;
    }

    [HttpGet("", Name = nameof(DataBinaryGetIndex))]
    public ActionResult<DataItem> DataBinaryGetIndex() {
        return this.Ok(
            this._Repository.GetAllDataItem(null));
    }

    [HttpGet("{name}", Name = nameof(GetByName))]
    public ActionResult GetByName(string name) {
        if (this._Repository.TryGetByName(name, out var item, out var _)) {
            return this.Ok(item);
        } else {
            return this.NotFound();
        }
    }

    [HttpPost("{name}", Name = nameof(DataBinaryPostByName))]
    public ActionResult DataBinaryPostByName(string name, [FromBody] byte[] data) {
        var contentType = this.Request.Headers.ContentType.FirstOrDefault() ?? string.Empty;
        this._Repository.Upsert(name, data, contentType);
        return this.Ok();
    }

    [HttpPut("{name}", Name = nameof(DataBinaryPutByName))]
    public async Task<ActionResult> DataBinaryPutByName(string name) {
        var contentType = this.Request.Headers.ContentType.FirstOrDefault() ?? string.Empty;
        var ms = new MemoryStream();
        await this.Request.BodyReader.CopyToAsync(ms);
        ms.Position = 0;
        var data = ms.ToArray();
        var upsert = this._Repository.Upsert(name, data, contentType);
        if (upsert.dataItem is null) {
            return this.Conflict();
        }
        return this.CreatedAtRoute(nameof(GetByName), new { name = name }, upsert.dataItem.AsDataItem());
    }

    [HttpDelete("{name}", Name = nameof(DataBinaryDeleteByName))]
    public ActionResult DataBinaryDeleteByName(string name) {
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