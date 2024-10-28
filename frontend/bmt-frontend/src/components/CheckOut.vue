<template>
  <v-main class="flex-grow-1">
    <v-container>
      <!-- Paso 1: Fecha y Hora de Entrega -->
      <v-card class="mb-5" flat>
        <v-card-title>
          <h2>Paso 1: Fecha y Hora de Entrega</h2>
        </v-card-title>
        <v-card-text>
          <v-form ref="step1Form">
            <v-container>
              <v-row v-for="product in cartProducts" :key="product.id" class="mb-4">
                <v-col cols="12">
                  <h3>{{ product.name }}</h3>
                </v-col>
                <!-- Fecha de Entrega para Productos Perecederos -->
                <v-col cols="12" md="6" v-if="product.type === 'Perishable'">
                  <v-menu v-model="product.menu" :close-on-content-click="false" transition="scale-transition" offset-y
                    min-width="auto">
                    <template v-slot:activator="{ on, attrs }">
                      <v-text-field v-model="product.deliveryDate" label="Fecha de entrega" prepend-icon="mdi-calendar"
                        readonly v-bind="attrs" v-on="on" :rules="[v => !!v || 'Seleccione una fecha de entrega']"
                        outlined></v-text-field>
                    </template>
                    <v-date-picker v-model="product.deliveryDate" @input="product.menu = false"
                      :allowed-dates="date => isAllowedDate(date, product.weekDaysAvailable)" locale="es"
                      :first-day-of-week="1" color="primary" header-color="primary">
                      <v-spacer></v-spacer>
                      <v-btn text color="primary" @click="product.menu = false">Cancelar</v-btn>
                      <v-btn text color="primary" @click="product.menu = false">OK</v-btn>
                    </v-date-picker>
                  </v-menu>
                </v-col>
                <!-- Fecha de Entrega Automática para Productos No Perecederos -->
                <v-col cols="12" md="6" v-else>
                  <v-text-field label="Fecha de entrega" :value="product.deliveryDate" readonly outlined></v-text-field>
                </v-col>
                <!-- Hora de Entrega para Productos Perecederos -->
                <v-col cols="12" md="6" v-if="product.type === 'Perishable'">
                  <v-menu v-model="product.menuTime" :close-on-content-click="false" transition="scale-transition"
                    offset-y min-width="auto">
                    <template v-slot:activator="{ on, attrs }">
                      <v-text-field v-model="product.deliveryTime" label="Hora de entrega" prepend-icon="mdi-clock"
                        readonly v-bind="attrs" v-on="on" :rules="[v => !!v || 'Seleccione una hora de entrega']"
                        outlined></v-text-field>
                    </template>
                    <v-time-picker v-model="product.deliveryTime" @input="product.menuTime = false" format="24hr"
                      color="primary" header-color="primary">
                      <v-spacer></v-spacer>
                      <v-btn text color="primary" @click="product.menuTime = false">Cancelar</v-btn>
                      <v-btn text color="primary" @click="product.menuTime = false">OK</v-btn>
                    </v-time-picker>
                  </v-menu>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card-text>
      </v-card>

      <!-- Paso 2: Dirección de Entrega -->
      <v-card class="mb-5" flat>
        <v-card-title>
          <h2>Paso 2: Dirección de Entrega</h2>
        </v-card-title>
        <v-card-text>
          <!-- Botón para Mostrar/Ocultar las Direcciones -->
          <v-btn color="secondary" text @click="toggleAddressMenu" class="mb-4">
            {{ isAddressOpen ? 'Ocultar Direcciones' : 'Mostrar Direcciones' }}
          </v-btn>

          <!-- Menú Desplegable de Direcciones -->
          <v-expand-transition>
            <div v-show="isAddressOpen">
              <v-row>
                <v-col v-for="(direction, index) in directions" :key="index" cols="12" class="mb-3">
                  <v-card class="mx-auto custom-card" width="100%" max-width="700" elevation="5" hover
                    @click="selectAddress(direction)"
                    :class="{ 'selected-card': selectedAddress && selectedAddress.id === direction.id }">
                    <v-card-item>
                      <v-card-title>{{ direction.numDirection }}</v-card-title>
                      <v-card-subtitle>
                        {{ direction.coordinates }}
                      </v-card-subtitle>
                    </v-card-item>
                    <v-divider></v-divider>
                    <v-card-text>{{ direction.otherSigns }}</v-card-text>
                  </v-card>
                </v-col>
              </v-row>
            </div>
          </v-expand-transition>

          <!-- Dirección Seleccionada -->
          <v-row v-if="selectedAddress" class="mt-4">
            <v-col cols="12">
              <v-card flat>
                <v-card-title>
                  <h3>Dirección Seleccionada:</h3>
                </v-card-title>
                <v-card-text>
                  <p><strong>Dirección:</strong> {{ selectedAddress.numDirection }}</p>
                  <p><strong>Coordenadas:</strong> {{ selectedAddress.coordinates }}</p>
                  <p><strong>Señales Adicionales:</strong> {{ selectedAddress.otherSigns }}</p>
                </v-card-text>
              </v-card>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <!-- Paso 3: Método de Pago -->
      <v-card class="mb-5" flat>
        <v-card-title>
          <h2>Paso 3: Método de Pago</h2>
        </v-card-title>
        <v-card-text>
          <v-form ref="step3Form">
            <v-radio-group v-model="paymentMethod" :rules="[v => !!v || 'Seleccione un método de pago']" row>
              <v-radio label="Tarjeta de crédito" value="credit-card"></v-radio>
              <v-radio label="Sinpe" value="sinpe"></v-radio>
            </v-radio-group>
          </v-form>
        </v-card-text>
      </v-card>

      <!-- Paso 4: Confirmación -->
      <v-card flat>
        <v-card-title>
          <h2>Paso 4: Confirmación</h2>
        </v-card-title>
        <v-card-text>
          <v-list>
            <v-list-item v-for="product in cartProducts" :key="product.id">
              <v-list-item-content>
                <v-list-item-title>{{ product.name }}</v-list-item-title>
                <v-list-item-subtitle>
                  Fecha de entrega: {{ product.deliveryDate }} <br>
                  Hora de entrega: {{ product.deliveryTime }}
                </v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
            <v-divider></v-divider>

            <!-- Mostrar Dirección de Entrega Solo Si Está Seleccionada -->
            <v-list-item v-if="selectedAddress">
              <v-list-item-content>
                <v-list-item-title>Dirección de entrega</v-list-item-title>
                <v-list-item-subtitle>
                  {{ selectedAddress.numDirection }}, {{ selectedAddress.coordinates }}<br>
                  {{ selectedAddress.otherSigns }}
                </v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
            <v-divider></v-divider>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>Método de pago</v-list-item-title>
                <v-list-item-subtitle>{{ paymentMethod }}</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-card-text>
        <v-card-actions>
          <v-btn text @click="scrollToStep(3)">Anterior</v-btn>
          <v-spacer></v-spacer>
          <v-btn color="success" @click="submitForm">Confirmar</v-btn>
        </v-card-actions>
      </v-card>

      <!-- Snackbar para Notificaciones -->
      <v-snackbar v-model="snackbar.show" :color="snackbar.color" timeout="3000">
        {{ snackbar.message }}
        <v-btn text @click="snackbar.show = false">Cerrar</v-btn>
      </v-snackbar>
    </v-container>
  </v-main>
</template>



<script>
import axios from 'axios';
import { API_URL } from '@/main';
import { getToken } from '@/helpers/auth'; // Asegúrate de tener esta función definida para obtener el token de autenticación

export default {
  data() {
    return {
      step: 1,
      paymentMethod: '',
      userId: '',
      shoppingCartId: '',
      cartProducts: [],
      directions: [],
      isAddressOpen: false,
      selectedAddress: null,
      snackbar: {
        show: false,
        message: '',
        color: '',
      },
    };
  },
  created() {
    const user = JSON.parse(localStorage.getItem('user')) || {};
    this.userId = user.id;

    // Obtener el carrito de compras
    axios
      .get(`${API_URL}/ShoppingCart`, { params: { userId: this.userId } })
      .then((response) => {
        const shoppingCart = response.data;
        this.shoppingCartId = shoppingCart.id;
        this.cartProducts = shoppingCart.cartProducts.map((item) => {
          console.log('Producto:', item); // Para depuración
          return {
            ...item,
            subtotal: item.subtotal.toFixed(2),
            newQuantity: item.quantity,
            deliveryDate: item.type === 'NoPerishable' ? this.getNextFriday() : '',
            deliveryTime: '',
            menu: false,
            menuTime: false,
          };
        });
        console.log('Cart Products:', this.cartProducts); // Para depuración adicional
      })
      .catch((error) => {
        console.error('Error fetching shopping cart', error);
        this.showToast('Error al obtener el carrito de compras', 'error');
      });

    // Obtener las direcciones del usuario
    this.GetDirectionsOfUser();
  },
  methods: {
    // Función para mostrar notificaciones
    showToast(message, color = 'error') {
      this.snackbar.message = message;
      this.snackbar.color = color;
      this.snackbar.show = true;
    },

    async GetDirectionsOfUser() {
      const token = getToken();
      const user = JSON.parse(localStorage.getItem('user'));
      try {
        if (!user || !user.id) {
          throw new Error('El usuario no tiene todos los campos requeridos');
        }
        const response = await axios.post(
          API_URL + '/Direction/ObtainDirectionsFromUser',
          user,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        this.directions = response.data;
      } catch (error) {
        console.error('Error al obtener las direcciones del usuario:', error);
      }
    },

    // Función para reiniciar el formulario
    resetForm() {
      this.step = 1;
      this.paymentMethod = '';
      this.selectedAddress = null;
      this.cartProducts.forEach((product) => {
        product.deliveryDate = product.type === 'NoPerishable' ? this.getNextFriday() : '';
        product.deliveryTime = '';
        product.menu = false;
        product.menuTime = false;
      });
      window.scrollTo({ top: 0, behavior: 'smooth' });
    },

    // Función para obtener el próximo viernes
    getNextFriday() {
      const today = new Date();
      const dayOfWeek = today.getDay();
      const daysUntilFriday = (5 - dayOfWeek + 7) % 7 || 7; // 5 representa viernes
      const nextFriday = new Date();
      nextFriday.setDate(today.getDate() + daysUntilFriday);
      return nextFriday.toISOString().split('T')[0];
    },

    // Función para verificar si una fecha es permitida según los días de la semana disponibles
    isAllowedDate(date, weekDaysAvailable) {
      if (!weekDaysAvailable) {
        // Si no se define, permitir todas las fechas
        return true;
      }
      const selectedDate = new Date(date);
      const dayOfWeek = selectedDate.getDay(); // 0 (Domingo) a 6 (Sábado)
      return weekDaysAvailable.includes(dayOfWeek.toString());
    },

    // Función para seleccionar una dirección
    selectAddress(direction) {
      this.selectedAddress = direction;
      this.isAddressOpen = false;
      this.showToast(`Dirección seleccionada: ${direction.numDirection}`, 'success');
    },

    // Función para alternar la visibilidad del menú de direcciones
    toggleAddressMenu() {
      this.isAddressOpen = !this.isAddressOpen;
    },

    // Función para manejar el envío del formulario
    submitForm() {
      // Validar formularios
      const step1Valid = this.$refs.step1Form.validate();
      const step3Valid = this.$refs.step3Form.validate();

      if (!step1Valid || !step3Valid || !this.selectedAddress) {
        this.showToast('Por favor, complete todos los campos requeridos.', 'error');
        return;
      }

      // Preparar datos para el envío
      const orderData = {
        userId: this.userId,
        shoppingCartId: this.shoppingCartId,
        products: this.cartProducts.map((product) => ({
          id: product.id,
          deliveryDate: product.deliveryDate,
          deliveryTime: product.deliveryTime,
        })),
        deliveryAddressId: this.selectedAddress.id,
        paymentMethod: this.paymentMethod,
      };

      // Enviar la orden al backend
      axios
        .post(`${API_URL}/Order/SubmitOrder`, orderData, {
          headers: {
            Authorization: `Bearer ${getToken()}`,
          },
        })
        .then(() => {
          this.showToast('Orden realizada con éxito!', 'success');
          this.resetForm();
        })
        .catch((error) => {
          console.error('Error al enviar la orden:', error);
          this.showToast('Error al enviar la orden', 'error');
        });
    },
  },
};
</script>

<style lang="scss" scoped>
/* Estilos personalizados */

/* Títulos de las cards */
.v-card-title h2 {
  margin: 0;
}

/* Margen inferior para separación de cards */
.mb-5 {
  margin-bottom: 2rem !important;
}

/* Margen inferior para filas */
.mb-3 {
  margin-bottom: 1rem !important;
}

/* Estilo para los títulos de los productos */
h3 {
  margin-bottom: 0.5rem;
}

/* Estilo para la tarjeta seleccionada */
.selected-card {
  border: 2px solid #1976D2;
  /* Azul predeterminado de Vuetify */
  background-color: #e3f2fd;
  /* Fondo ligeramente azul para resaltar */
}

/* Estilos adicionales para resaltar mejor la tarjeta seleccionada */
.custom-card:hover {
  background-color: #f5f5f5;
  transition: background-color 0.3s ease;
}
</style>
