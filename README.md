[![NuGet](https://img.shields.io/nuget/v/imagekit.svg)]()
# DotNET (NET45/Core2.0) SDK v2.x for ImageKit 

New version of the DotNet SDK for [ImageKit.io](https://imagekit.io) that implements the new APIs and interface for performing different file operations.

**Note**: For older version of SDK, please checkout branch v1.x.

ImageKit is a complete image optimization and transformation solution that comes with an [image CDN](https://imagekit.io/features/imagekit-infrastructure) and media storage. It can be integrated with your existing infrastructure - storages like AWS S3, web servers, your CDN and custom domain names, allowing you to deliver optimized images in minutes with minimal code changes.


## Installation

Package Manager
```
Install-Package Imagekit
```

PackageReference
```
<PackageReference Include="Imagekit" Version="3.0.1">
</PackageReference>
```
.Net CLI
```
dotnet add package Imagekit --version 3.0.1
```


Open up your project, navigate to Nuget package manager console and add Imagekit package.
Also you can search for [Imagekit](https://www.nuget.org/packages/Imagekit) in Nuget GUI.


## Initialization
Add this reference where you want to use imagekit.io services:
```cs
using Imagekit;

Imagekit.Imagekit imagekit = new Imagekit.Imagekit(publicKey, privateKey, urlEndPoint, "path");
```

**Note**: You can get the apiKey, apiSecret and ImagekitId from your [Imagekit.io dashboard](https://imagekit.io/dashboard).

## Usage
You can use this DotNET SDK for 3 different kinds of functions - URL generation, file uploads and file management. The usage of the SDK has been explained below

### URL Generation

**1. Using image path and image hostname or endpoint**

This method allows you to create a URL using the `path` where the image exists and the URL endpoint (`urlEndpoint`) you want to use to access the image. You can refer to the documentation [here](https://docs.imagekit.io/imagekit-docs/url-endpoints) to read more about URL endpoints in ImageKit and the section about [image origins](https://docs.imagekit.io/imagekit-docs/configure-origin) to understand about paths with different kinds of origins.

```
string imageURL = imagekit.Url(new Transformation().Width(400).Height(300))
.Path("/default-image.jpg")
.UrlEndpoint("https://ik.imagekit.io/your_imagekit_id/endpoint")
.TransformationPosition("query")
.Generate();
```

This results in a URL like

```
https://ik.imagekit.io/your_imagekit_id/endpoint/tr:h-300,w-400/default-image.jpg
```

**2. Using full image URL**

This method allows you to add transformation parameters to an existing, complete URL that is already mapped to ImageKit using the `src` parameter. This method should be used if you have the complete URL mapped to ImageKit stored in your database.


```
string imageURL = imagekit.Url(new Transformation().Width(400).Height(300))
.Src("https://ik.imagekit.io/your_imagekit_id/endpoint/default-image.jpg")
.Generate();
```

This results in a URL like

```
https://ik.imagekit.io/your_imagekit_id/endpoint/default-image.jpg?tr=h-300,w-400
```


The `.Url()` method accepts the following parameters

| Option           | Description                    |
| :----------------| :----------------------------- |
| urlEndpoint      | Optional. The base URL to be appended before the path of the image. If not specified, the URL Endpoint specified at the time of SDK initialization is used. For example, https://ik.imagekit.io/your_imagekit_id/endpoint/ |
| path             | Conditional. This is the path at which the image exists. For example, `/path/to/image.jpg`. Either the `path` or `src` parameter need to be specified for URL generation. |
| src              | Conditional. This is the complete URL of an image already mapped to ImageKit. For example, `https://ik.imagekit.io/your_imagekit_id/endpoint/path/to/image.jpg`. Either the `path` or `src` parameter need to be specified for URL generation. |
| transformation   | Optional. An array of objects specifying the transformation to be applied in the URL. The transformation name  and the value should be specified as a key-value pair in the object. Different steps of a [chained transformation](https://docs.imagekit.io/imagekit-docs/chained-transformations) can be specified as different objects of the array. The complete list of supported transformations in the SDK and some examples of using them are given later. If you use a transformation name that is not specified in the SDK, it gets applied as it is in the URL. |
| transformationPostion | Optional. Default value is `path` that places the transformation string as a path parameter in the URL. Can also be specified as `query` which adds the transformation string as the query parameter `tr` in the URL. If you use `src` parameter to create the URL, then the transformation string is always added as a query parameter. |
| queryParameters  | Optional. These are the other query parameters that you want to add to the final URL. These can be any query parameters and not necessarily related to ImageKit. Especially useful, if you want to add some versioning parameter to your URLs. |
| signed           | Optional. Boolean. Default is `false`. If set to `true`, the SDK generates a signed image URL adding the image signature to the image URL. This can only be used if you are creating the URL with the `urlEndpoint` and `path` parameters, and not with the `src` parameter. |
| expireSeconds    | Optional. Integer. Meant to be used along with the `signed` parameter to specify the time in seconds from now when the URL should expire. If specified, the URL contains the expiry timestamp in the URL and the image signature is modified accordingly. |



#### Examples of generating URLs

**1. Chained Transformations as a query parameter**
```
Transformation transformation = new Transformation()
    .Width(400).Height(300)
    .Chain()
    .Rotation(90);

string imageURL = imagekit.Url(transformation)
    .Path("/default-image.jpg")
    .UrlEndpoint("https://ik.imagekit.io/your_imagekit_id/endpoint")
    .TransformationPosition("query")
    .Generate();
```

```
https://ik.imagekit.io/your_imagekit_id/endpoint/default-image.jpg?tr=h-300,w-400:rt-90
```

**2. Sharpening and contrast transforms and a progressive JPG image**

There are some transforms like [Sharpening](https://docs.imagekit.io/imagekit-docs/image-enhancement---color-manipulation) that can be added to the URL with or without any other value. 

```cs
string src = "https://ik.imagekit.io/your_imagekit_id/endpoint/default-image.jpg";

Transformation trans = new Transformation().Format("jpg")
    .Progressive(true)
    .EffectSharpen()
    .EffectContrast(1);

string imageURL = imagekit.Url(trans)
.Src(src)
.Generate();
```
**Note**: Because `src` parameter was used, the transformation string gets added as a query parameter `tr`.

```
https://ik.imagekit.io/your_imagekit_id/endpoint/default-image.jpg?tr=f-jpg,pr-true,e-sharpen,e-contrast-1
```


**3. Signed URL that expires in 300 seconds with the default URL endpoint and other query parameters**
```cs
Transformation trans = new Transformation()
    .Height(300).Width(400);
string[] queryParams = { "v=123" };

string imageURL = imagekit.Url(trans)
    .Path("/default-image.jpg")
    .QueryParameters(queryParams)
    .Signed(true).ExpireSeconds(300)
    .Generate();
```
```
https://ik.imagekit.io/your_imagekit_id/tr:h-300,w-400/default-image.jpg?v=123&ik-t=1567358667&ik-s=f2c7cdacbe7707b71a83d49cf1c6110e3d701054
```

#### List of supported transformations

The complete list of transformations supported and their usage in ImageKit can be found [here](https://docs.imagekit.io/imagekit-docs/image-transformations). The SDK gives a name to each transformation parameter, making the code simpler and readable. If a transformation is supported in ImageKit, but a name for it cannot be found in the table below, then use the transformation code from ImageKit docs as the name when using in the `url` function.

| Supported Transformation Name | Translates to parameter |
|-------------------------------|-------------------------|
| HEIGHT | h |
| WIDTH | w |
| ASPECT_RATIO | ar |
| QUALITY | q |
| CROP | c |
| CROP_MODE | cm |
| X | x |
| Y | y |
| FOCUS | fo |
| FORMAT | f |
| RADIUS | r |
| BACKGROUND | bg |
| BORDER | bo |
| ROTATION | rt |
| BLUR | bl |
| NAMED | n |
| OVERLAY_IMAGE | oi |
| OVERLAY_X | ox |
| OVERLAY_Y | oy |
| OVERLAY_FOCUS | ofo |
| OVERLAY_HEIGHT | oh |
| OVERLAY_WIDTH | ow |
| OVERLAY_TEXT | ot |
| OVERLAY_TEXT_FONT_SIZE | ots |
| OVERLAY_TEXT_FONT_FAMILY | otf |
| OVERLAY_TEXT_COLOR | otc |
| OVERLAY_ALPHA | oa |
| OVERLAY_TEXT_TYPOGRAPHY | ott |
| OVERLAY_BACKGROUND | obg |
| OVERLAY_IMAGE_TRIM | oit |
| PROGRESSIVE | pr |
| LOSSLESS | lo |
| TRIM | t |
| METADATA | md |
| COLOR_PROFILE | cp |
| DEFAULT_IMAGE | di |
| DPR | dpr |
| EFFECT_SHARPEN | e-sharpen |
| EFFECT_USM | e-usm |
| EFFECT_CONTRAST | e-contrast |
| EFFECT_GRAY | e-grayscale |
| ORIGINAL | orig |



### File Upload

The SDK provides a simple interface using the `.upload()` method to upload files to the ImageKit Media Library. It accepts all the parameters supported by the [ImageKit Upload API](https://docs.imagekit.io/imagekit-docs/server-side-file-upload).

The `upload()` method requires at least the `file` and the `fileName` parameter to upload a file. You can pass other parameters supported by the ImageKit upload API using the same parameter name as specified in the upload API documentation. For example, to specify tags for a file at the time of upload use the `tags` parameter as specified in the [documentation here](https://docs.imagekit.io/imagekit-docs/server-side-file-upload).

Sample usage
```cs
ImagekitResponse resp = imagekit.FileName("my_file_name.jpg")
    .Upload(<url|base_64|binary>);
});
```

**Note**: Upload argument can be a URL or byte array (byte[]) or Base64String of a file.


### File Management

The SDK provides a simple interface for all the [media APIs mentioned here](https://docs.imagekit.io/imagekit-docs/media-api) to manage your files. 

**1. List & Search Files**

Accepts an object specifying the parameters to be used to list and search files. All parameters specified in the [documentation here](https://docs.imagekit.io/imagekit-docs/list-and-search-files-api) can be passed as is with the correct values to get the results.

```cs
List<ListAPIResponse> resp = imagekit.Skip(0).Limit(10).ListFiles();
```

**2. Get File Details**

Accepts the file ID and fetches the details as per the [API documentation here](https://docs.imagekit.io/imagekit-docs/get-file-details-api).

```cs
ListAPIResponse resp = imagekit.GetFileDetails(fileId);
```

**3. Get File Metadata**

Accepts the file ID and fetches the metadata as per the [API documentation here](https://docs.imagekit.io/imagekit-docs/get-image-metadata-api).

```cs
MetadataResponse resp = imagekit.GetFileMetadata(fileId);
```

**4. Update File Details**

Update parameters associated with the file as per the [API documentation here](https://docs.imagekit.io/imagekit-docs/update-file-details-api). 

```cs
string[] tags = { "one", "two" };
ListAPIResponse resp = imagekit.Tags(tags).CustomCoordinates("10,10,100,100").UpdateFileDetails(fileId);
```

**5. Delete File**

Delete a file as per the [API documentation here](https://docs.imagekit.io/imagekit-docs/delete-file-api). The method accepts the file ID of the file that has to be deleted.

```cs
string resp = imagekit.DeleteFile(fileId);
```

**6. Purge Cache**

Programmatically issue a cache clear request as per the [API documentation here](https://docs.imagekit.io/imagekit-docs/purge-cache-api). Accepts the full URL of the file for which the cache has to be cleared.

```cs
var PurgeAPIResponse = imagekit.PurgeCache("full_url");
```

**7. Purge Cache Status**

Get the purge cache request status using the request ID returned when a purge cache request gets submitted as per the [API documentation here](https://docs.imagekit.io/imagekit-docs/purge-cache-status-api)

```cs
PurgeCacheStatusResponse resp = imagekit.GetPurgeCacheStatus("cache_request_id");
```

### Authentication Parameter Generation

In case you are looking to implement client-side file upload, you are going to need a token, expiry timestamp and a valid signature for that upload. The SDK provides a simple method that you can use in your code to generate these authentication parameters for you.

*Note: The Private API Key should never be exposed in any client-side code. You must always generate these authentication parameters on the server-side*

```
AuthParamResponse resp = imagekit.GetAuthenticationParameters();
```

Returns
```
{
    token : "unique_token",
    expire : "valid_expiry_timestamp",
    signature : "generated_signature"
}
```

Both the `token` and `expire` parameters are optional. If not specified the SDK uses the [uuid](https://www.npmjs.com/package/uuid) package to generate a random token and also generates a valid expiry timestamp internally. The value of the `token` and `expire` used to generate the signature are always returned in the response, no matter if they are provided as an input to this method or not.

## Support

For any feedback or to report any issues or general implementation support please reach out to [support@imagekit.io](mailto:support@imagekit.io)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details


