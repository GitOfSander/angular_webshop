import { ModuleWithProviders, Injectable } from '@angular/core';

@Injectable()
export class ObjectService {
    constructor() { }

    getArrayByCallName(result: any, value: string): any {
        for (var i in result) {
            if (result[i].callName === value) {
                return result[i];
            }
        }

        return "";
    }

    getCompressedPathFromDataBundle(result: any, value: string): any {
        for (var i in result.dataTemplateUploads) {
            if (result.dataTemplateUploads[i].callName === value) {
                return result.dataItemFiles.find((dataItemFile: any) => dataItemFile.dataTemplateUploadId == result.dataTemplateUploads[i].id).compressedPath;
            }
        }

        return "";
    }

    getCompressedPathFromPageBundle(result: any, value: string): any {
        for (var i in result.pageTemplateUploads) {
            if (result.pageTemplateUploads[i].callName === value) {
                return result.pageFiles.find((pageFile: any) => pageFile.pageTemplateUploadId == result.pageTemplateUploads[i].id).compressedPath;
            }
        }

        return "";
    }

    getCompressedPathFromWebsiteBundle(result: any, value: string): any {
        for (var i in result.websiteUploads) {
            if (result.websiteUploads[i].callName === value) {
                return result.websiteFiles.find((websiteFile: any) => websiteFile.websiteUploadId == result.websiteUploads[i].id).compressedPath;
            }
        }

        return "";
    }

    getDataItemFileFromDataBundle(result: any, value: string): any {
        for (var i in result.dataTemplateUploads) {
            if (result.dataTemplateUploads[i].callName === value) {
                return result.dataItemFiles.find((dataItemFile: any) => dataItemFile.dataTemplateUploadId == result.dataTemplateUploads[i].id);
            }
        }

        return "";
    }

    getOriginalPathFromDataBundle(result: any, value: string): any {
        for (var i in result.dataTemplateUploads) {
            if (result.dataTemplateUploads[i].callName === value) {
                return result.dataItemFiles.find((dataItemFile: any) => dataItemFile.dataTemplateUploadId == result.dataTemplateUploads[i].id).originalPath;
            }
        }

        return "";
    }

    getOriginalPathFromPageBundle(result: any, value: string): any {
        for (var i in result.pageTemplateUploads) {
            if (result.pageTemplateUploads[i].callName === value) {
                return result.pageFiles.find((pageFile: any) => pageFile.pageTemplateUploadId == result.pageTemplateUploads[i].id).originalPath;
            }
        }

        return "";
    }

    getOriginaldPathFromWebsiteBundle(result: any, value: string): any {
        for (var i in result.websiteUploads) {
            if (result.websiteUploads[i].callName === value) {
                return result.websiteFiles.find((websiteFile: any) => websiteFile.websiteUploadId == result.websiteUploads[i].id).originalPath;
            }
        }

        return "";
    }

    getTextFromDataBundle(result: any, value: string): any {
        for (var i in result.dataTemplateFields) {
            if (result.dataTemplateFields[i].callName === value) {
                return result.dataItemResources.find((dataItemResource: any) => dataItemResource.dataTemplateFieldId == result.dataTemplateFields[i].id).text;
            }
        }

        return "";
    }

    getTextFromPageBundle(result: any, value: string): any {
        for (var i in result.pageTemplateFields) {
            if (result.pageTemplateFields[i].callName === value) {
                return result.pageResources.find((pageResource: any) => pageResource.pageTemplateFieldId == result.pageTemplateFields[i].id).text;
            }
        }

        return "";
    }

    getTextFromWebsiteBundle(result: any, value: string): any {
        for (var i in result.websiteFields) {
            if (result.websiteFields[i].callName === value) {
                return result.websiteResources.find((websiteResource: any) => websiteResource.websiteFieldId == result.websiteFields[i].id).text;
            }
        }

        return "";
    }
}