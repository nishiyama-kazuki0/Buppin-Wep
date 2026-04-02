function Download(fileName, content) {
    const file = new File([content], fileName, { type: "application/octet-stream" });
    const exportUrl = URL.createObjectURL(file);

    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = fileName;
    a.target = "_self";
    a.click();

    URL.revokeObjectURL(exportUrl);
}