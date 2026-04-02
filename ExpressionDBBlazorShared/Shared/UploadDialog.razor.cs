using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen.Blazor;

namespace ExpressionDBBlazorShared.Shared;

public partial class UploadDialog : ComponentBase
{
    [Inject]
    private CommonService? ComService { get; set; }
    //EventConsole console;

    private RadzenUpload upload = new();

    private void OnChange(UploadChangeEventArgs args, string name)
    {
        foreach (Radzen.FileInfo? file in args.Files)
        {
            // console.Log($"File: {file.Name} / {file.Size} bytes");
        }

        //console.Log($"{name} changed");
    }

    private void OnProgress(UploadProgressArgs args, string name)
    {
        //console.Log($"{args.Progress}% '{name}' / {args.Loaded} of {args.Total} bytes.");

        if (args.Progress == 100)
        {
            foreach (Radzen.FileInfo? file in args.Files)
            {
                //console.Log($"Uploaded: {file.Name} / {file.Size} bytes");
            }
        }
    }

    private void OnClick(MouseEventArgs args)
    {
        upload.Url = WebComService.GetUploadUrl();
        _ = upload.Upload();
    }

    private void CompleteUpload(UploadCompleteEventArgs args)
    {
        if (!args.Cancelled)
        {
            DialogService.CloseSide(args.RawResponse);
        }
        else
        {
            DialogService.CloseSide(string.Empty);
        }
    }
}
