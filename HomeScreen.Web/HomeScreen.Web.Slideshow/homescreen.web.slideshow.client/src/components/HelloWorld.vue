<template>
  <div class="weather-component">
    <h1>Weather forecast</h1>
    <p>This component demonstrates fetching data from the server.</p>

    <div v-if="loading" class="loading">
      Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationvue">https://aka.ms/jspsintegrationvue</a>
      for more details.
    </div>

    <div v-if="current" class="content">
      <table>
        <thead>
        <tr>
          <th>Feels Like</th>
          <th>Min Temperature</th>
          <th>Max Temperature</th>
          <th>Change of Rain</th>
          <th>Amount of Rain</th>
          <th>Weather Code</th>
        </tr>
        </thead>
        <tbody>
        <tr>
          <td>{{ current.feelsLikeTemperature }}</td>
          <td>{{ current.minTemperature }}</td>
          <td>{{ current.maxTemperature }}</td>
          <td>{{ current.chanceOfRain }}</td>
          <td>{{ current.amountOfRain }}</td>
          <td>{{ current.weatherCode }}</td>
        </tr>
        </tbody>
      </table>
    </div>

    <div v-if="daily" class="content">
      <table>
        <thead>
        <tr>
          <th>Time</th>
          <th>Apparent Temperature Min</th>
          <th>Apparent Temperature Max</th>
          <th>Daylight Duration</th>
          <th>Sunrise</th>
          <th>Sunset</th>
          <th>UV Index Clear Sky Max</th>
          <th>UV Index Max</th>
          <th>Weather Code</th>
          <th>Weather Code Label</th>
          <th>Precipitation Sum</th>
          <th>Precipitation Probability Max</th>
          <th>Precipitation Probability Min</th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="item in daily" :key="item.time?.toMillis()">
          <td>{{ item.time?.toHTTP() }}</td>
          <td>{{ item.apparentTemperatureMin }}</td>
          <td>{{ item.apparentTemperatureMax }}</td>
          <td>{{ item.daylightDuration }}</td>
          <td>{{ item.sunrise }}</td>
          <td>{{ item.sunset }}</td>
          <td>{{ item.uvIndexClearSkyMax }}</td>
          <td>{{ item.uvIndexMax }}</td>
          <td>{{ item.weatherCode }}</td>
          <td>{{ item.weatherCodeLabel }}</td>
          <td>{{ item.precipitationSum }}</td>
          <td>{{ item.precipitationProbabilityMax }}</td>
          <td>{{ item.precipitationProbabilityMin }}</td>
        </tr>
        </tbody>
      </table>
    </div>

    <div v-if="hourly" class="content">
      <table>
        <thead>
        <tr>
          <th>Time</th>
          <th>Apparent Temperature</th>
          <th>Precipitation</th>
          <th>Precipitation Probability</th>
          <th>Wind Direction</th>
          <th>Wind Speed</th>
          <th>Wind Gusts</th>
          <th>Is Day</th>
          <th>Cloud Cover</th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="item in hourly" :key="item.time?.toMillis()">
          <td>{{ item.time?.toHTTP() }}</td>
          <td>{{ item.apparentTemperature }}</td>
          <td>{{ item.precipitation }}</td>
          <td>{{ item.precipitationProbability }}</td>
          <td>{{ item.windDirection }}</td>
          <td>{{ item.windSpeed }}</td>
          <td>{{ item.windGusts }}</td>
          <td>{{ item.isDay }}</td>
          <td>{{ item.cloudCover }}</td>
        </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script lang="ts">
import {defineComponent} from 'vue';
import {
  WeatherForecastClient,
  type IWeatherForecastClient, type IWeatherForecast, type IDailyForecast, type IHourlyForecast
} from '@/domain/api/homescreen-slideshow-api'

interface Data {
  loading: boolean,
  current: null | IWeatherForecast,
  daily: null | IDailyForecast[],
  hourly: null | IHourlyForecast[],
  client: IWeatherForecastClient,
  longitude: number,
  latitude: number,
}

export default defineComponent({
  data(): Data {
    return {
      loading: false,
      current: null,
      daily: null,
      hourly: null,
      client: new WeatherForecastClient(""),
      longitude: 151.10149000,
      latitude: -33.96770000,
    };
  },
  created() {
    // fetch the data when the view is created and the data is
    // already being observed
    this.fetchData();
  },
  watch: {
    // call again the method if the route changes
    '$route': 'fetchData'
  },
  methods: {
    fetchData(): void {
      this.current = null;
      this.loading = true;
      this.client.getCurrentForecast(this.longitude, this.latitude).then((response) => {
        this.current = response.result;
      }).then(async () => await this.client.getHourlyForecast(this.longitude, this.latitude)).then((response) => {
        this.hourly = response.result;
      }).then(async () => await this.client.getDailyForecast(this.longitude, this.latitude)).then((response) => {
        this.daily = response.result;
        this.loading = false;
      });
    }
  },
});
</script>

<style scoped>
th {
  font-weight: bold;
}

th, td {
  padding-left: .5rem;
  padding-right: .5rem;
}

.weather-component {
  text-align: center;
}

table {
  margin-left: auto;
  margin-right: auto;
}
</style>