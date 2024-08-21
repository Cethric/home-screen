//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v14.1.0.0 (NJsonSchema v11.0.2.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming

import { DateTime, Duration } from "luxon";

export interface IConfigClient {

    config(): Promise<SwaggerResponse<Config>>;
}

export class ConfigClient implements IConfigClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl ?? "";
    }

    config(signal?: AbortSignal): Promise<SwaggerResponse<Config>> {
        let url_ = this.baseUrl + "/api/config";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            signal,
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processConfig(_response);
        });
    }

    protected processConfig(response: Response): Promise<SwaggerResponse<Config>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        let _mappings: { source: any, target: any }[] = [];
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : jsonParse(_responseText, this.jsonParseReviver);
            result200 = Config.fromJS(resultData200, _mappings);
            return new SwaggerResponse(status, _headers, result200);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<Config>>(new SwaggerResponse(status, _headers, null as any));
    }
}

export interface IMediaClient {

    random(count: number): Promise<SwaggerResponse<MediaItem>>;

    toggle(mediaId: string, enabled: boolean): Promise<SwaggerResponse<MediaItem>>;

    download(mediaId: string, width: number, height: number, blur: boolean, format: MediaTransformOptionsFormat): Promise<SwaggerResponse<void>>;

    transform(mediaId: string, width: number, height: number, blur: boolean, format: MediaTransformOptionsFormat): Promise<SwaggerResponse<AcceptedTransformMeta>>;
}

export class MediaClient implements IMediaClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl ?? "";
    }

    random(count: number, signal?: AbortSignal): Promise<SwaggerResponse<MediaItem>> {
        let url_ = this.baseUrl + "/api/media/random?";
        if (count === undefined || count === null)
            throw new Error("The parameter 'count' must be defined and cannot be null.");
        else
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
            return this.processRandom(_response);
        });
    }

    protected processRandom(response: Response): Promise<SwaggerResponse<MediaItem>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        let _mappings: { source: any, target: any }[] = [];
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : jsonParse(_responseText, this.jsonParseReviver);
            result200 = MediaItem.fromJS(resultData200, _mappings);
            return new SwaggerResponse(status, _headers, result200);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<MediaItem>>(new SwaggerResponse(status, _headers, null as any));
    }

    toggle(mediaId: string, enabled: boolean, signal?: AbortSignal): Promise<SwaggerResponse<MediaItem>> {
        let url_ = this.baseUrl + "/api/media/{mediaId}/toggle?";
        if (mediaId === undefined || mediaId === null)
            throw new Error("The parameter 'mediaId' must be defined.");
        url_ = url_.replace("{mediaId}", encodeURIComponent("" + mediaId));
        if (enabled === undefined || enabled === null)
            throw new Error("The parameter 'enabled' must be defined and cannot be null.");
        else
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
            return this.processToggle(_response);
        });
    }

    protected processToggle(response: Response): Promise<SwaggerResponse<MediaItem>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        let _mappings: { source: any, target: any }[] = [];
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : jsonParse(_responseText, this.jsonParseReviver);
            result200 = MediaItem.fromJS(resultData200, _mappings);
            return new SwaggerResponse(status, _headers, result200);
            });
        } else if (status === 404) {
            return response.text().then((_responseText) => {
            return throwException("A server side error occurred.", status, _responseText, _headers);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<MediaItem>>(new SwaggerResponse(status, _headers, null as any));
    }

    download(mediaId: string, width: number, height: number, blur: boolean, format: MediaTransformOptionsFormat, signal?: AbortSignal): Promise<SwaggerResponse<void>> {
        let url_ = this.baseUrl + "/api/media/{mediaId}/download/{width}/{height}?";
        if (mediaId === undefined || mediaId === null)
            throw new Error("The parameter 'mediaId' must be defined.");
        url_ = url_.replace("{mediaId}", encodeURIComponent("" + mediaId));
        if (width === undefined || width === null)
            throw new Error("The parameter 'width' must be defined.");
        url_ = url_.replace("{width}", encodeURIComponent("" + width));
        if (height === undefined || height === null)
            throw new Error("The parameter 'height' must be defined.");
        url_ = url_.replace("{height}", encodeURIComponent("" + height));
        if (blur === undefined || blur === null)
            throw new Error("The parameter 'blur' must be defined and cannot be null.");
        else
            url_ += "blur=" + encodeURIComponent("" + blur) + "&";
        if (format === undefined || format === null)
            throw new Error("The parameter 'format' must be defined and cannot be null.");
        else
            url_ += "format=" + encodeURIComponent("" + format) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            signal,
            headers: {
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processDownload(_response);
        });
    }

    protected processDownload(response: Response): Promise<SwaggerResponse<void>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 404) {
            return response.text().then((_responseText) => {
            return throwException("A server side error occurred.", status, _responseText, _headers);
            });
        } else if (status === 400) {
            return response.text().then((_responseText) => {
            return throwException("A server side error occurred.", status, _responseText, _headers);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<void>>(new SwaggerResponse(status, _headers, null as any));
    }

    transform(mediaId: string, width: number, height: number, blur: boolean, format: MediaTransformOptionsFormat, signal?: AbortSignal): Promise<SwaggerResponse<AcceptedTransformMeta>> {
        let url_ = this.baseUrl + "/api/media/{mediaId}/transform/{width}/{height}?";
        if (mediaId === undefined || mediaId === null)
            throw new Error("The parameter 'mediaId' must be defined.");
        url_ = url_.replace("{mediaId}", encodeURIComponent("" + mediaId));
        if (width === undefined || width === null)
            throw new Error("The parameter 'width' must be defined.");
        url_ = url_.replace("{width}", encodeURIComponent("" + width));
        if (height === undefined || height === null)
            throw new Error("The parameter 'height' must be defined.");
        url_ = url_.replace("{height}", encodeURIComponent("" + height));
        if (blur === undefined || blur === null)
            throw new Error("The parameter 'blur' must be defined and cannot be null.");
        else
            url_ += "blur=" + encodeURIComponent("" + blur) + "&";
        if (format === undefined || format === null)
            throw new Error("The parameter 'format' must be defined and cannot be null.");
        else
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
            return this.processTransform(_response);
        });
    }

    protected processTransform(response: Response): Promise<SwaggerResponse<AcceptedTransformMeta>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        let _mappings: { source: any, target: any }[] = [];
        if (status === 404) {
            return response.text().then((_responseText) => {
            return throwException("A server side error occurred.", status, _responseText, _headers);
            });
        } else if (status === 202) {
            return response.text().then((_responseText) => {
            let result202: any = null;
            let resultData202 = _responseText === "" ? null : jsonParse(_responseText, this.jsonParseReviver);
            result202 = AcceptedTransformMeta.fromJS(resultData202, _mappings);
            return new SwaggerResponse(status, _headers, result202);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<AcceptedTransformMeta>>(new SwaggerResponse(status, _headers, null as any));
    }
}

export interface IWeatherClient {

    current(longitude: number, latitude: number): Promise<SwaggerResponse<WeatherForecast>>;

    hourly(longitude: number, latitude: number): Promise<SwaggerResponse<HourlyForecast[]>>;

    daily(longitude: number, latitude: number): Promise<SwaggerResponse<DailyForecast[]>>;
}

export class WeatherClient implements IWeatherClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl ?? "";
    }

    current(longitude: number, latitude: number, signal?: AbortSignal): Promise<SwaggerResponse<WeatherForecast>> {
        let url_ = this.baseUrl + "/api/weather/{longitude}/{latitude}/current";
        if (longitude === undefined || longitude === null)
            throw new Error("The parameter 'longitude' must be defined.");
        url_ = url_.replace("{longitude}", encodeURIComponent("" + longitude));
        if (latitude === undefined || latitude === null)
            throw new Error("The parameter 'latitude' must be defined.");
        url_ = url_.replace("{latitude}", encodeURIComponent("" + latitude));
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            signal,
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processCurrent(_response);
        });
    }

    protected processCurrent(response: Response): Promise<SwaggerResponse<WeatherForecast>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        let _mappings: { source: any, target: any }[] = [];
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : jsonParse(_responseText, this.jsonParseReviver);
            result200 = WeatherForecast.fromJS(resultData200, _mappings);
            return new SwaggerResponse(status, _headers, result200);
            });
        } else if (status === 404) {
            return response.text().then((_responseText) => {
            return throwException("A server side error occurred.", status, _responseText, _headers);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<WeatherForecast>>(new SwaggerResponse(status, _headers, null as any));
    }

    hourly(longitude: number, latitude: number, signal?: AbortSignal): Promise<SwaggerResponse<HourlyForecast[]>> {
        let url_ = this.baseUrl + "/api/weather/{longitude}/{latitude}/hourly";
        if (longitude === undefined || longitude === null)
            throw new Error("The parameter 'longitude' must be defined.");
        url_ = url_.replace("{longitude}", encodeURIComponent("" + longitude));
        if (latitude === undefined || latitude === null)
            throw new Error("The parameter 'latitude' must be defined.");
        url_ = url_.replace("{latitude}", encodeURIComponent("" + latitude));
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            signal,
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processHourly(_response);
        });
    }

    protected processHourly(response: Response): Promise<SwaggerResponse<HourlyForecast[]>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        let _mappings: { source: any, target: any }[] = [];
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : jsonParse(_responseText, this.jsonParseReviver);
            if (Array.isArray(resultData200)) {
                result200 = [] as any;
                for (let item of resultData200)
                    result200!.push(HourlyForecast.fromJS(item, _mappings));
            }
            else {
                result200 = <any>null;
            }
            return new SwaggerResponse(status, _headers, result200);
            });
        } else if (status === 404) {
            return response.text().then((_responseText) => {
            return throwException("A server side error occurred.", status, _responseText, _headers);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<HourlyForecast[]>>(new SwaggerResponse(status, _headers, null as any));
    }

    daily(longitude: number, latitude: number, signal?: AbortSignal): Promise<SwaggerResponse<DailyForecast[]>> {
        let url_ = this.baseUrl + "/api/weather/{longitude}/{latitude}/daily";
        if (longitude === undefined || longitude === null)
            throw new Error("The parameter 'longitude' must be defined.");
        url_ = url_.replace("{longitude}", encodeURIComponent("" + longitude));
        if (latitude === undefined || latitude === null)
            throw new Error("The parameter 'latitude' must be defined.");
        url_ = url_.replace("{latitude}", encodeURIComponent("" + latitude));
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            signal,
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processDaily(_response);
        });
    }

    protected processDaily(response: Response): Promise<SwaggerResponse<DailyForecast[]>> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        let _mappings: { source: any, target: any }[] = [];
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : jsonParse(_responseText, this.jsonParseReviver);
            if (Array.isArray(resultData200)) {
                result200 = [] as any;
                for (let item of resultData200)
                    result200!.push(DailyForecast.fromJS(item, _mappings));
            }
            else {
                result200 = <any>null;
            }
            return new SwaggerResponse(status, _headers, result200);
            });
        } else if (status === 404) {
            return response.text().then((_responseText) => {
            return throwException("A server side error occurred.", status, _responseText, _headers);
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SwaggerResponse<DailyForecast[]>>(new SwaggerResponse(status, _headers, null as any));
    }
}

export class Config implements IConfig {
    mediaUrl?: string;

    constructor(data?: IConfig) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any, _mappings?: any) {
        if (_data) {
            this.mediaUrl = _data["mediaUrl"];
        }
    }

    static fromJS(data: any, _mappings?: any): Config | null {
        data = typeof data === 'object' ? data : {};
        return createInstance<Config>(data, _mappings, Config);
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["mediaUrl"] = this.mediaUrl;
        return data;
    }
}

export interface IConfig {
    mediaUrl?: string;
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

    init(_data?: any, _mappings?: any) {
        if (_data) {
            this.id = _data["id"];
            this.created = _data["created"] ? DateTime.fromISO(_data["created"].toString()) : <any>undefined;
            this.notes = _data["notes"];
            this.enabled = _data["enabled"];
            this.location = _data["location"] ? MediaItemLocation.fromJS(_data["location"], _mappings) : <any>undefined;
        }
    }

    static fromJS(data: any, _mappings?: any): MediaItem | null {
        data = typeof data === 'object' ? data : {};
        return createInstance<MediaItem>(data, _mappings, MediaItem);
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

    init(_data?: any, _mappings?: any) {
        if (_data) {
            this.name = _data["name"];
            this.latitude = _data["latitude"];
            this.longitude = _data["longitude"];
        }
    }

    static fromJS(data: any, _mappings?: any): MediaItemLocation | null {
        data = typeof data === 'object' ? data : {};
        return createInstance<MediaItemLocation>(data, _mappings, MediaItemLocation);
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

/** 0 = Jpeg 1 = JpegXL 2 = Png 3 = WebP 4 = Avif */
export enum MediaTransformOptionsFormat {
    Jpeg = 0,
    JpegXL = 1,
    Png = 2,
    WebP = 3,
    Avif = 4,
}

export class AcceptedTransformMeta implements IAcceptedTransformMeta {
    mediaId?: string;
    width?: number;
    height?: number;
    blur?: boolean;
    format?: MediaTransformOptionsFormat;

    constructor(data?: IAcceptedTransformMeta) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any, _mappings?: any) {
        if (_data) {
            this.mediaId = _data["mediaId"];
            this.width = _data["width"];
            this.height = _data["height"];
            this.blur = _data["blur"];
            this.format = _data["format"];
        }
    }

    static fromJS(data: any, _mappings?: any): AcceptedTransformMeta | null {
        data = typeof data === 'object' ? data : {};
        return createInstance<AcceptedTransformMeta>(data, _mappings, AcceptedTransformMeta);
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["mediaId"] = this.mediaId;
        data["width"] = this.width;
        data["height"] = this.height;
        data["blur"] = this.blur;
        data["format"] = this.format;
        return data;
    }
}

export interface IAcceptedTransformMeta {
    mediaId?: string;
    width?: number;
    height?: number;
    blur?: boolean;
    format?: MediaTransformOptionsFormat;
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

    init(_data?: any, _mappings?: any) {
        if (_data) {
            this.feelsLikeTemperature = _data["feelsLikeTemperature"];
            this.maxTemperature = _data["maxTemperature"];
            this.minTemperature = _data["minTemperature"];
            this.chanceOfRain = _data["chanceOfRain"];
            this.amountOfRain = _data["amountOfRain"];
            this.weatherCode = _data["weatherCode"];
        }
    }

    static fromJS(data: any, _mappings?: any): WeatherForecast | null {
        data = typeof data === 'object' ? data : {};
        return createInstance<WeatherForecast>(data, _mappings, WeatherForecast);
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

    init(_data?: any, _mappings?: any) {
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

    static fromJS(data: any, _mappings?: any): HourlyForecast | null {
        data = typeof data === 'object' ? data : {};
        return createInstance<HourlyForecast>(data, _mappings, HourlyForecast);
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

    init(_data?: any, _mappings?: any) {
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

    static fromJS(data: any, _mappings?: any): DailyForecast | null {
        data = typeof data === 'object' ? data : {};
        return createInstance<DailyForecast>(data, _mappings, DailyForecast);
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

/** 0 = Clear 1 = MostlyClear 2 = PartlyClear 3 = Overcast 45 = Fog 48 = RimeFog 51 = LightDrizzle 53 = MediumDrizzle 55 = HeavyDrizzle 56 = LightFreezingDrizzle 57 = HeavyFreezingDrizzle 61 = LightRain 63 = MediumRain 65 = HeavyRain 66 = LightFreezingRain 67 = HeavyFreezingRain 71 = LightSnow 73 = MediumSnow 75 = HeavySnow 77 = GrainySnow 80 = LightRainShower 81 = MediumRainShower 82 = HeavyRainShower 85 = LightSnowShower 86 = HeavySnowShower 95 = Thunderstorm 96 = ThunderstormWithSomeRain 99 = ThunderstormWithHeavyRain */
export enum WmoWeatherCode {
    Clear = 0,
    MostlyClear = 1,
    PartlyClear = 2,
    Overcast = 3,
    Fog = 45,
    RimeFog = 48,
    LightDrizzle = 51,
    MediumDrizzle = 53,
    HeavyDrizzle = 55,
    LightFreezingDrizzle = 56,
    HeavyFreezingDrizzle = 57,
    LightRain = 61,
    MediumRain = 63,
    HeavyRain = 65,
    LightFreezingRain = 66,
    HeavyFreezingRain = 67,
    LightSnow = 71,
    MediumSnow = 73,
    HeavySnow = 75,
    GrainySnow = 77,
    LightRainShower = 80,
    MediumRainShower = 81,
    HeavyRainShower = 82,
    LightSnowShower = 85,
    HeavySnowShower = 86,
    Thunderstorm = 95,
    ThunderstormWithSomeRain = 96,
    ThunderstormWithHeavyRain = 99,
}

function jsonParse(json: any, reviver?: any) {
    json = JSON.parse(json, reviver);

    var byid: any = {};
    var refs: any = [];
    json = (function recurse(obj: any, prop?: any, parent?: any) {
        if (typeof obj !== 'object' || !obj)
            return obj;
        
        if ("$ref" in obj) {
            let ref = obj.$ref;
            if (ref in byid)
                return byid[ref];
            refs.push([parent, prop, ref]);
            return undefined;
        } else if ("$id" in obj) {
            let id = obj.$id;
            delete obj.$id;
            if ("$values" in obj)
                obj = obj.$values;
            byid[id] = obj;
        }
        
        if (Array.isArray(obj)) {
            obj = obj.map((v, i) => recurse(v, i, obj));
        } else {
            for (var p in obj) {
                if (obj.hasOwnProperty(p) && obj[p] && typeof obj[p] === 'object')
                    obj[p] = recurse(obj[p], p, obj);
            }
        }

        return obj;
    })(json);

    for (let i = 0; i < refs.length; i++) {
        const ref = refs[i];
        ref[0][ref[1]] = byid[ref[2]];
    }

    return json;
}

function createInstance<T>(data: any, mappings: any, type: any): T | null {
  if (!mappings)
    mappings = [];
  if (!data)
    return null;

  const mappingIndexName = "__mappingIndex";
  if (data[mappingIndexName])
    return <T>mappings[data[mappingIndexName]].target;

  data[mappingIndexName] = mappings.length;

  let result: any = new type();
  mappings.push({ source: data, target: result });
  result.init(data, mappings);
  return result;
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