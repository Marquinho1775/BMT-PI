<template>
  <v-app class="d-flex flex-column">
    <AppHeader />
    <v-main class="flex-grow-1">
      <v-container>
        <v-card class="pa-5 mb-4">
          <v-row>
            <v-col cols="12" sm="12" md="8" class="mx-auto">
              <h3 class="text-center">Registrar Dirección</h3>

              <v-form @submit.prevent="registerAddress" @reset="onReset">
                <v-text-field
                  label="Nombre de la dirección"
                  v-model="addressData.numDirection"
                  placeholder="Ingrese el nombre"
                  required
                ></v-text-field>

                <!-- Google Maps API Interactive Map -->
                <v-row class="my-4">
                  <v-col>
                    <GMapMap
                      :center="mapCenter"
                      :zoom="12"
                      style="width: 100%; height: 400px"
                      @click="addMarker"
                    >
                      <Marker
                        v-for="(marker, index) in markers"
                        :key="index"
                        :position="marker.position"
                      />
                    </GMapMap>
                  </v-col>
                </v-row>

                <v-text-field
                  label="Coordenadas"
                  v-model="addressData.coordinates"
                  placeholder="Las coordenadas se completarán automáticamente"
                  readonly
                ></v-text-field>

                <v-text-field
                  label="Otras señas"
                  v-model="addressData.otherSigns"
                  placeholder="Ingrese otras señas o referencias"
                ></v-text-field>

                <div class="d-flex justify-content-between">
                  <v-btn color="secondary" @click="goBack">Volver</v-btn>
                  <v-btn type="submit" color="primary">Registrar Dirección</v-btn>
                </div>
              </v-form>
            </v-col>
          </v-row>
        </v-card>
      </v-container>
    </v-main>
    <AppFooter />
    <AppSidebar />
  </v-app>
</template>

<script>
import axios from 'axios';
import { API_URL } from '@/main.js';
import { GMapMap, Marker } from '@fawmi/vue-google-maps';

export default {
  components: {
    GMapMap,
    Marker,
  },
  data() {
    return {
      addressData: {
        username: JSON.parse(localStorage.getItem('user')).username,
        numDirection: '',
        coordinates: '',
        otherSigns: '',
      },
      mapCenter: { lat: 9.935901499999423, lng: -84.05064582824707 }, // Default coordinates, Universidad de Costa Rica
      markers: [],
    };
  },
  methods: {
    async registerAddress() {
      try {
        const response = await axios.post(`${API_URL}/Direction/CreateDirection`, this.addressData);
        console.log(response);
        await this.$swal.fire({
          title: 'Registro exitoso',
          text: '¡Su dirección ha sido registrada correctamente!',
          icon: 'success',
          confirmButtonText: 'Ok',
        });
        this.$router.push('/profile');
      } catch (error) {
        console.error('Error al registrar la dirección:', error);
        this.$swal.fire({
          title: 'Error',
          text: 'Hubo un error al registrar su dirección. Inténtelo de nuevo.',
          icon: 'error',
          confirmButtonText: 'Ok',
        });
      }
    },
    addMarker(event) {
      const { latLng } = event;
      const position = { lat: latLng.lat(), lng: latLng.lng() };
      this.markers = [{ position }];
      this.addressData.coordinates = `${position.lat}, ${position.lng}`;
    },
    onReset(event) {
      event.preventDefault();
      this.addressData.numDirection = '';
      this.addressData.coordinates = '';
      this.addressData.otherSigns = '';
      this.markers = [];
    },
    goBack() {
      this.$router.push('/profile');
    },
  },
};
</script>

<style scoped>
.v-app-bar {
  background-color: #9FC9FC;
}

.v-footer {
  height: 50px;
  background-color: #9FC9FC;
}

.v-card {
  background-color: #ffffff;
}

.text-center {
  text-align: center;
}

.my-4 {
  margin-top: 20px;
  margin-bottom: 20px;
}
</style>
