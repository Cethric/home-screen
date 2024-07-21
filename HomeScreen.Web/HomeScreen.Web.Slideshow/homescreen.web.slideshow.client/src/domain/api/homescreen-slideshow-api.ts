//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v14.1.0.0 (NJsonSchema v11.0.2.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming

import { DateTime, Duration } from "luxon";

export interface IMediaClient {

    getRandomMediaItems(count?: number | undefined): Promise<SwaggerResponse<MediaItem>>;

    toggleMediaItem(id?: string | undefined, enabled?: boolean | undefined): Promise<SwaggerResponse<MediaItem>>;

    downloadMediaItem(id?: string | undefined, width?: number | undefined, height?: number | undefined, blur?: boolean | undefined, format?: MediaTransformOptionsFormat | undefined): Promise<SwaggerResponse<FileResponse>>;

    getTransformMediaItemUrl(id?: string | undefined, width?: number | undefined, height?: number | undefined, blur?: boolean | undefined, format?: MediaTransformOptionsFormat | undefined): Promise<SwaggerResponse<string>>;
}

export class MediaClient implements IMediaClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl ?? "";
    }

    getRandomMediaItems(count?: number | undefined, signal?: AbortSignal): Promise<SwaggerResponse<MediaItem>> {
        let url_ = this.baseUrl + "/api/Media/GetRandomMediaItems?";
        if (count === null)
            throw new Error("The parameter 'count' cannot be null.");
        else if (count !== undefined)
            url_ += "count=" + encodeURIComponent("" + count) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            signal,
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processGetRandomMediaItems(_response);
        });
    }

    protected processGetRandomMediaItems(response: Response): Promise<SwaggerResponse<MediaItem>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = MediaItem.fromJS(resultData200);
            return new SwaggerResponse(status, _headers, result200);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<MediaItem>>(new SwaggerResponse(status, _headers, null as any));
    }

    toggleMediaItem(id?: string | undefined, enabled?: boolean | undefined, signal?: AbortSignal): Promise<SwaggerResponse<MediaItem>> {
        let url_ = this.baseUrl + "/api/Media/ToggleMediaItem?";
        if (id === null)
            throw new Error("The parameter 'id' cannot be null.");
        else if (id !== undefined)
            url_ += "id=" + encodeURIComponent("" + id) + "&";
        if (enabled === null)
            throw new Error("The parameter 'enabled' cannot be null.");
        else if (enabled !== undefined)
            url_ += "enabled=" + encodeURIComponent("" + enabled) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "PATCH",
            signal,
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processToggleMediaItem(_response);
        });
    }

    protected processToggleMediaItem(response: Response): Promise<SwaggerResponse<MediaItem>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 202) {
            return response.text().then((_responseText) => {
            let result202: any = null;
            let resultData202 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result202 = MediaItem.fromJS(resultData202);
            return new SwaggerResponse(status, _headers, result202);
            });
        } else if (status === 404) {
            return response.text().then((_responseText) => {
            let result404: any = null;
            let resultData404 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
                result404 = resultData404 !== undefined ? resultData404 : <any>null;
    
            return throwException("A server side error occurred.", status, _responseText, _headers, result404);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<MediaItem>>(new SwaggerResponse(status, _headers, null as any));
    }

    downloadMediaItem(id?: string | undefined, width?: number | undefined, height?: number | undefined, blur?: boolean | undefined, format?: MediaTransformOptionsFormat | undefined, signal?: AbortSignal): Promise<SwaggerResponse<FileResponse>> {
        let url_ = this.baseUrl + "/api/Media/DownloadMediaItem?";
        if (id === null)
            throw new Error("The parameter 'id' cannot be null.");
        else if (id !== undefined)
            url_ += "id=" + encodeURIComponent("" + id) + "&";
        if (width === null)
            throw new Error("The parameter 'width' cannot be null.");
        else if (width !== undefined)
            url_ += "width=" + encodeURIComponent("" + width) + "&";
        if (height === null)
            throw new Error("The parameter 'height' cannot be null.");
        else if (height !== undefined)
            url_ += "height=" + encodeURIComponent("" + height) + "&";
        if (blur === null)
            throw new Error("The parameter 'blur' cannot be null.");
        else if (blur !== undefined)
            url_ += "blur=" + encodeURIComponent("" + blur) + "&";
        if (format === null)
            throw new Error("The parameter 'format' cannot be null.");
        else if (format !== undefined)
            url_ += "format=" + encodeURIComponent("" + format) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            signal,
            headers: {
                "Accept": "application/octet-stream"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processDownloadMediaItem(_response);
        });
    }

    protected processDownloadMediaItem(response: Response): Promise<SwaggerResponse<FileResponse>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200 || status === 206) {
            const contentDisposition = response.headers ? response.headers.get("content-disposition") : undefined;
            let fileNameMatch = contentDisposition ? /filename\*=(?:(\\?['"])(.*?)\1|(?:[^\s]+'.*?')?([^;\n]*))/g.exec(contentDisposition) : undefined;
            let fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[3] || fileNameMatch[2] : undefined;
            if (fileName) {
                fileName = decodeURIComponent(fileName);
            } else {
                fileNameMatch = contentDisposition ? /filename="?([^"]*?)"?(;|$)/g.exec(contentDisposition) : undefined;
                fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[1] : undefined;
            }
            return response.blob().then(blob => { return new SwaggerResponse(status, _headers, { fileName: fileName, data: blob, status: status, headers: _headers }); });
        } else if (status === 404) {
            return response.text().then((_responseText) => {
            let result404: any = null;
            let resultData404 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
                result404 = resultData404 !== undefined ? resultData404 : <any>null;
    
            return throwException("A server side error occurred.", status, _responseText, _headers, result404);
            });
        } else if (status === 400) {
            return response.text().then((_responseText) => {
            let result400: any = null;
            let resultData400 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
                result400 = resultData400 !== undefined ? resultData400 : <any>null;
    
            return throwException("A server side error occurred.", status, _responseText, _headers, result400);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<FileResponse>>(new SwaggerResponse(status, _headers, null as any));
    }

    getTransformMediaItemUrl(id?: string | undefined, width?: number | undefined, height?: number | undefined, blur?: boolean | undefined, format?: MediaTransformOptionsFormat | undefined, signal?: AbortSignal): Promise<SwaggerResponse<string>> {
        let url_ = this.baseUrl + "/api/Media/GetTransformMediaItemUrl?";
        if (id === null)
            throw new Error("The parameter 'id' cannot be null.");
        else if (id !== undefined)
            url_ += "id=" + encodeURIComponent("" + id) + "&";
        if (width === null)
            throw new Error("The parameter 'width' cannot be null.");
        else if (width !== undefined)
            url_ += "width=" + encodeURIComponent("" + width) + "&";
        if (height === null)
            throw new Error("The parameter 'height' cannot be null.");
        else if (height !== undefined)
            url_ += "height=" + encodeURIComponent("" + height) + "&";
        if (blur === null)
            throw new Error("The parameter 'blur' cannot be null.");
        else if (blur !== undefined)
            url_ += "blur=" + encodeURIComponent("" + blur) + "&";
        if (format === null)
            throw new Error("The parameter 'format' cannot be null.");
        else if (format !== undefined)
            url_ += "format=" + encodeURIComponent("" + format) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            signal,
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processGetTransformMediaItemUrl(_response);
        });
    }

    protected processGetTransformMediaItemUrl(response: Response): Promise<SwaggerResponse<string>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 202) {
            return response.text().then((_responseText) => {
            let result202: any = null;
            let resultData202 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
                result202 = resultData202 !== undefined ? resultData202 : <any>null;
    
            return new SwaggerResponse(status, _headers, result202);
            });
        } else if (status === 404) {
            return response.text().then((_responseText) => {
            let result404: any = null;
            let resultData404 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
                result404 = resultData404 !== undefined ? resultData404 : <any>null;
    
            return throwException("A server side error occurred.", status, _responseText, _headers, result404);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<string>>(new SwaggerResponse(status, _headers, null as any));
    }
}

export interface IWeatherForecastClient {

    getCurrentForecast(longitude?: number | undefined, latitude?: number | undefined): Promise<SwaggerResponse<WeatherForecast>>;

    getHourlyForecast(longitude?: number | undefined, latitude?: number | undefined): Promise<SwaggerResponse<HourlyForecast[]>>;

    getDailyForecast(longitude?: number | undefined, latitude?: number | undefined): Promise<SwaggerResponse<DailyForecast[]>>;
}

export class WeatherForecastClient implements IWeatherForecastClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl ?? "";
    }

    getCurrentForecast(longitude?: number | undefined, latitude?: number | undefined, signal?: AbortSignal): Promise<SwaggerResponse<WeatherForecast>> {
        let url_ = this.baseUrl + "/api/WeatherForecast/GetCurrentForecast?";
        if (longitude === null)
            throw new Error("The parameter 'longitude' cannot be null.");
        else if (longitude !== undefined)
            url_ += "longitude=" + encodeURIComponent("" + longitude) + "&";
        if (latitude === null)
            throw new Error("The parameter 'latitude' cannot be null.");
        else if (latitude !== undefined)
            url_ += "latitude=" + encodeURIComponent("" + latitude) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            signal,
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processGetCurrentForecast(_response);
        });
    }

    protected processGetCurrentForecast(response: Response): Promise<SwaggerResponse<WeatherForecast>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = WeatherForecast.fromJS(resultData200);
            return new SwaggerResponse(status, _headers, result200);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<WeatherForecast>>(new SwaggerResponse(status, _headers, null as any));
    }

    getHourlyForecast(longitude?: number | undefined, latitude?: number | undefined, signal?: AbortSignal): Promise<SwaggerResponse<HourlyForecast[]>> {
        let url_ = this.baseUrl + "/api/WeatherForecast/GetHourlyForecast?";
        if (longitude === null)
            throw new Error("The parameter 'longitude' cannot be null.");
        else if (longitude !== undefined)
            url_ += "longitude=" + encodeURIComponent("" + longitude) + "&";
        if (latitude === null)
            throw new Error("The parameter 'latitude' cannot be null.");
        else if (latitude !== undefined)
            url_ += "latitude=" + encodeURIComponent("" + latitude) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            signal,
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processGetHourlyForecast(_response);
        });
    }

    protected processGetHourlyForecast(response: Response): Promise<SwaggerResponse<HourlyForecast[]>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (Array.isArray(resultData200)) {
                result200 = [] as any;
                for (let item of resultData200)
                    result200!.push(HourlyForecast.fromJS(item));
            }
            else {
                result200 = <any>null;
            }
            return new SwaggerResponse(status, _headers, result200);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<HourlyForecast[]>>(new SwaggerResponse(status, _headers, null as any));
    }

    getDailyForecast(longitude?: number | undefined, latitude?: number | undefined, signal?: AbortSignal): Promise<SwaggerResponse<DailyForecast[]>> {
        let url_ = this.baseUrl + "/api/WeatherForecast/GetDailyForecast?";
        if (longitude === null)
            throw new Error("The parameter 'longitude' cannot be null.");
        else if (longitude !== undefined)
            url_ += "longitude=" + encodeURIComponent("" + longitude) + "&";
        if (latitude === null)
            throw new Error("The parameter 'latitude' cannot be null.");
        else if (latitude !== undefined)
            url_ += "latitude=" + encodeURIComponent("" + latitude) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            signal,
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processGetDailyForecast(_response);
        });
    }

    protected processGetDailyForecast(response: Response): Promise<SwaggerResponse<DailyForecast[]>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (Array.isArray(resultData200)) {
                result200 = [] as any;
                for (let item of resultData200)
                    result200!.push(DailyForecast.fromJS(item));
            }
            else {
                result200 = <any>null;
            }
            return new SwaggerResponse(status, _headers, result200);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<DailyForecast[]>>(new SwaggerResponse(status, _headers, null as any));
    }
}

export class MediaItem implements IMediaItem {
    id?: string;
    created?: DateTime;
    notes?: string;
    enabled?: boolean;
    location?: MediaItemLocation;

    constructor(data?: IMediaItem) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
            this.location = data.location && !(<any>data.location).toJSON ? new MediaItemLocation(data.location) : <MediaItemLocation>this.location;
        }
    }

    init(_data?: any) {
        if (_data) {
            this.id = _data["id"];
            this.created = _data["created"] ? DateTime.fromISO(_data["created"].toString()) : <any>undefined;
            this.notes = _data["notes"];
            this.enabled = _data["enabled"];
            this.location = _data["location"] ? MediaItemLocation.fromJS(_data["location"]) : <any>undefined;
        }
    }

    static fromJS(data: any): MediaItem {
        data = typeof data === 'object' ? data : {};
        let result = new MediaItem();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["id"] = this.id;
        data["created"] = this.created ? this.created.toString() : <any>undefined;
        data["notes"] = this.notes;
        data["enabled"] = this.enabled;
        data["location"] = this.location ? this.location.toJSON() : <any>undefined;
        return data;
    }
}

export interface IMediaItem {
    id?: string;
    created?: DateTime;
    notes?: string;
    enabled?: boolean;
    location?: IMediaItemLocation;
}

export class MediaItemLocation implements IMediaItemLocation {
    name?: string;
    latitude?: number;
    longitude?: number;

    constructor(data?: IMediaItemLocation) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.name = _data["name"];
            this.latitude = _data["latitude"];
            this.longitude = _data["longitude"];
        }
    }

    static fromJS(data: any): MediaItemLocation {
        data = typeof data === 'object' ? data : {};
        let result = new MediaItemLocation();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["name"] = this.name;
        data["latitude"] = this.latitude;
        data["longitude"] = this.longitude;
        return data;
    }
}

export interface IMediaItemLocation {
    name?: string;
    latitude?: number;
    longitude?: number;
}

export enum MediaTransformOptionsFormat {
    Jpeg = "Jpeg",
    WebP = "WebP",
    Avif = "Avif",
}

export class WeatherForecast implements IWeatherForecast {
    feelsLikeTemperature?: number;
    maxTemperature?: number;
    minTemperature?: number;
    chanceOfRain?: number;
    amountOfRain?: number;
    weatherCode?: string;

    constructor(data?: IWeatherForecast) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.feelsLikeTemperature = _data["feelsLikeTemperature"];
            this.maxTemperature = _data["maxTemperature"];
            this.minTemperature = _data["minTemperature"];
            this.chanceOfRain = _data["chanceOfRain"];
            this.amountOfRain = _data["amountOfRain"];
            this.weatherCode = _data["weatherCode"];
        }
    }

    static fromJS(data: any): WeatherForecast {
        data = typeof data === 'object' ? data : {};
        let result = new WeatherForecast();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["feelsLikeTemperature"] = this.feelsLikeTemperature;
        data["maxTemperature"] = this.maxTemperature;
        data["minTemperature"] = this.minTemperature;
        data["chanceOfRain"] = this.chanceOfRain;
        data["amountOfRain"] = this.amountOfRain;
        data["weatherCode"] = this.weatherCode;
        return data;
    }
}

export interface IWeatherForecast {
    feelsLikeTemperature?: number;
    maxTemperature?: number;
    minTemperature?: number;
    chanceOfRain?: number;
    amountOfRain?: number;
    weatherCode?: string;
}

export class HourlyForecast implements IHourlyForecast {
    time?: DateTime;
    apparentTemperature?: number;
    precipitation?: number;
    precipitationProbability?: number;
    windDirection?: number;
    windSpeed?: number;
    windGusts?: number;
    isDay?: boolean;
    cloudCover?: number;

    constructor(data?: IHourlyForecast) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.time = _data["time"] ? DateTime.fromISO(_data["time"].toString()) : <any>undefined;
            this.apparentTemperature = _data["apparentTemperature"];
            this.precipitation = _data["precipitation"];
            this.precipitationProbability = _data["precipitationProbability"];
            this.windDirection = _data["windDirection"];
            this.windSpeed = _data["windSpeed"];
            this.windGusts = _data["windGusts"];
            this.isDay = _data["isDay"];
            this.cloudCover = _data["cloudCover"];
        }
    }

    static fromJS(data: any): HourlyForecast {
        data = typeof data === 'object' ? data : {};
        let result = new HourlyForecast();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["time"] = this.time ? this.time.toString() : <any>undefined;
        data["apparentTemperature"] = this.apparentTemperature;
        data["precipitation"] = this.precipitation;
        data["precipitationProbability"] = this.precipitationProbability;
        data["windDirection"] = this.windDirection;
        data["windSpeed"] = this.windSpeed;
        data["windGusts"] = this.windGusts;
        data["isDay"] = this.isDay;
        data["cloudCover"] = this.cloudCover;
        return data;
    }
}

export interface IHourlyForecast {
    time?: DateTime;
    apparentTemperature?: number;
    precipitation?: number;
    precipitationProbability?: number;
    windDirection?: number;
    windSpeed?: number;
    windGusts?: number;
    isDay?: boolean;
    cloudCover?: number;
}

export class DailyForecast implements IDailyForecast {
    time?: DateTime;
    apparentTemperatureMin?: number;
    apparentTemperatureMax?: number;
    daylightDuration?: number;
    sunrise?: DateTime;
    sunset?: DateTime;
    uvIndexClearSkyMax?: number;
    uvIndexMax?: number;
    weatherCode?: WmoWeatherCode;
    weatherCodeLabel?: string;
    precipitationSum?: number;
    precipitationProbabilityMax?: number;
    precipitationProbabilityMin?: number;

    constructor(data?: IDailyForecast) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.time = _data["time"] ? DateTime.fromISO(_data["time"].toString()) : <any>undefined;
            this.apparentTemperatureMin = _data["apparentTemperatureMin"];
            this.apparentTemperatureMax = _data["apparentTemperatureMax"];
            this.daylightDuration = _data["daylightDuration"];
            this.sunrise = _data["sunrise"] ? DateTime.fromISO(_data["sunrise"].toString()) : <any>undefined;
            this.sunset = _data["sunset"] ? DateTime.fromISO(_data["sunset"].toString()) : <any>undefined;
            this.uvIndexClearSkyMax = _data["uvIndexClearSkyMax"];
            this.uvIndexMax = _data["uvIndexMax"];
            this.weatherCode = _data["weatherCode"];
            this.weatherCodeLabel = _data["weatherCodeLabel"];
            this.precipitationSum = _data["precipitationSum"];
            this.precipitationProbabilityMax = _data["precipitationProbabilityMax"];
            this.precipitationProbabilityMin = _data["precipitationProbabilityMin"];
        }
    }

    static fromJS(data: any): DailyForecast {
        data = typeof data === 'object' ? data : {};
        let result = new DailyForecast();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["time"] = this.time ? this.time.toFormat('yyyy-MM-dd') : <any>undefined;
        data["apparentTemperatureMin"] = this.apparentTemperatureMin;
        data["apparentTemperatureMax"] = this.apparentTemperatureMax;
        data["daylightDuration"] = this.daylightDuration;
        data["sunrise"] = this.sunrise ? this.sunrise.toString() : <any>undefined;
        data["sunset"] = this.sunset ? this.sunset.toString() : <any>undefined;
        data["uvIndexClearSkyMax"] = this.uvIndexClearSkyMax;
        data["uvIndexMax"] = this.uvIndexMax;
        data["weatherCode"] = this.weatherCode;
        data["weatherCodeLabel"] = this.weatherCodeLabel;
        data["precipitationSum"] = this.precipitationSum;
        data["precipitationProbabilityMax"] = this.precipitationProbabilityMax;
        data["precipitationProbabilityMin"] = this.precipitationProbabilityMin;
        return data;
    }
}

export interface IDailyForecast {
    time?: DateTime;
    apparentTemperatureMin?: number;
    apparentTemperatureMax?: number;
    daylightDuration?: number;
    sunrise?: DateTime;
    sunset?: DateTime;
    uvIndexClearSkyMax?: number;
    uvIndexMax?: number;
    weatherCode?: WmoWeatherCode;
    weatherCodeLabel?: string;
    precipitationSum?: number;
    precipitationProbabilityMax?: number;
    precipitationProbabilityMin?: number;
}

export enum WmoWeatherCode {
    Clear = "Clear",
    MostlyClear = "MostlyClear",
    PartlyClear = "PartlyClear",
    Overcast = "Overcast",
    Fog = "Fog",
    RimeFog = "RimeFog",
    LightDrizzle = "LightDrizzle",
    MediumDrizzle = "MediumDrizzle",
    HeavyDrizzle = "HeavyDrizzle",
    LightFreezingDrizzle = "LightFreezingDrizzle",
    HeavyFreezingDrizzle = "HeavyFreezingDrizzle",
    LightRain = "LightRain",
    MediumRain = "MediumRain",
    HeavyRain = "HeavyRain",
    LightFreezingRain = "LightFreezingRain",
    HeavyFreezingRain = "HeavyFreezingRain",
    LightSnow = "LightSnow",
    MediumSnow = "MediumSnow",
    HeavySnow = "HeavySnow",
    GrainySnow = "GrainySnow",
    LightRainShower = "LightRainShower",
    MediumRainShower = "MediumRainShower",
    HeavyRainShower = "HeavyRainShower",
    LightSnowShower = "LightSnowShower",
    HeavySnowShower = "HeavySnowShower",
    Thunderstorm = "Thunderstorm",
    ThunderstormWithSomeRain = "ThunderstormWithSomeRain",
    ThunderstormWithHeavyRain = "ThunderstormWithHeavyRain",
}

export class SwaggerResponse<TResult> {
    status: number;
    headers: { [key: string]: any; };
    result: TResult;

    constructor(status: number, headers: { [key: string]: any; }, result: TResult)
    {
        this.status = status;
        this.headers = headers;
        this.result = result;
    }
}

export interface FileResponse {
    data: Blob;
    status: number;
    fileName?: string;
    headers?: { [name: string]: any };
}

export class ApiException extends Error {
    override message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isApiException = true;

    static isApiException(obj: any): obj is ApiException {
        return obj.isApiException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): any {
    throw new ApiException(message, status, response, headers, result);
}