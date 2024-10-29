<template>
  <v-main class="flex-grow-1">
    <v-container>
      <!-- PASO 1: Fecha de Entrega -->
      <v-card class="mb-5 pa-5 elevation-4" outlined>
        <v-card-title class="d-flex align-center">
          <v-icon large color="primary">mdi-calendar-check</v-icon>
          <h2 class="ml-3">Paso 1: Fecha de Entrega</h2>
        </v-card-title>
        <v-divider class="my-4"></v-divider>
        <v-card-text>
          <v-form ref="step1Form">
            <v-row>
              <v-col v-for="item in cartProducts" :key="item.id" cols="12" md="6" lg="4" class="mb-4">
                <v-card class="pa-3" outlined>
                  <v-img :src="item.imageURL || 'ruta/a/imagen-placeholder.jpg'" alt="Product Image"
                    class="product-image" aspect-ratio="1.75" contain></v-img>
                  <v-card-title class="justify-center mt-2">
                    <span class="text-h6">{{ item.product.name }}</span>
                  </v-card-title>
                  <v-card-subtitle class="text-center">
                    Subtotal: ${{ item.subtotal }}
                  </v-card-subtitle>
                  <v-divider class="my-2"></v-divider>
                  <v-btn v-if="item.product.type === 'Perishable'" color="secondary" @click="openDialog(item)"
                    class="ma-2" block>
                    <v-icon left>mdi-calendar</v-icon>
                    Seleccionar Fecha
                  </v-btn>
                  <v-dialog v-model="item.isDialogOpen" max-width="380px">
                    <v-card>
                      <v-card-title>
                        <span class="text-h6">Selecciona una Fecha</span>
                      </v-card-title>
                      <v-card-text>
                        <v-date-picker v-model="item.deliveryDate" @update:model-value="() => onDateChange(item)"
                          color="secondary" :min="getToday()"
                          :allowed-dates="(date) => allowedDates(date, item.product.weekDaysAvailable)"
                          locale="es"></v-date-picker>
                      </v-card-text>
                      <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn text color="secondary" @click="closeDialog(item)">
                          Cancelar
                        </v-btn>
                        <v-btn text color="secondary" @click="closeDialog(item)">
                          OK
                        </v-btn>
                      </v-card-actions>
                    </v-card>
                  </v-dialog>
                  <v-alert v-if="item.deliveryDate" type="success" class="mt-2" elevation="2" border="left">
                    <v-icon left color="green">mdi-check-circle</v-icon>
                    Fecha de Entrega Seleccionada: {{ item.deliveryDate }}
                  </v-alert>
                </v-card>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
      </v-card>

      <!-- PASO 2: Dirección de Entrega -->
      <v-card class="mb-5 pa-5 elevation-4" outlined>
        <v-card-title class="d-flex align-center">
          <v-icon large color="primary">mdi-map-marker</v-icon>
          <h2 class="ml-3">Paso 2: Dirección de Entrega</h2>
        </v-card-title>
        <v-divider class="my-4"></v-divider>
        <v-card-text>
          <v-btn color="secondary" text @click="toggleAddressMenu" class="mb-4" block>
            <v-icon left>mdi-arrow-down</v-icon>
            {{ isAddressOpen ? 'Ocultar Direcciones' : 'Mostrar Direcciones' }}
          </v-btn>
          <v-expand-transition>
            <div v-show="isAddressOpen">
              <v-row>
                <v-col v-for="(direction, index) in directions" :key="index" cols="12" class="mb-3">
                  <v-card class="mx-auto custom-card" width="100%" max-width="700" elevation="5" hover
                    @click="selectAddress(direction)"
                    :class="{ 'selected-card': selectedAddress && selectedAddress.id === direction.id }">
                    <v-card-item>
                      <v-icon color="primary" class="mr-2">mdi-map-marker-outline</v-icon>
                      <v-card-title>{{ direction.numDirection }}</v-card-title>
                    </v-card-item>
                    <v-divider></v-divider>
                    <v-card-text>
                      <p><strong>Coordenadas:</strong> {{ direction.coordinates }}</p>
                      <p><strong>Señales Adicionales:</strong> {{ direction.otherSigns }}</p>
                    </v-card-text>
                  </v-card>
                </v-col>
              </v-row>
            </div>
          </v-expand-transition>
          <v-alert v-if="selectedAddress" type="success" class="mt-4" elevation="2" border="left">
            <v-icon left color="green">mdi-check-circle</v-icon>
            <strong>Dirección Seleccionada:</strong> {{ selectedAddress.numDirection }}, {{ selectedAddress.coordinates
            }}
            <br>
            {{ selectedAddress.otherSigns }}
          </v-alert>
        </v-card-text>
      </v-card>

      <!-- PASO 3: Método de Pago -->
      <v-card class="mb-5 pa-5 elevation-4" outlined>
        <v-card-title class="d-flex align-center">
          <v-icon large color="primary">mdi-cash</v-icon>
          <h2 class="ml-3">Paso 3: Método de Pago</h2>
        </v-card-title>
        <v-divider class="my-4"></v-divider>
        <v-card-text>
          <v-form ref="step3Form">
            <v-radio-group v-model="paymentMethod" :rules="[v => !!v || 'Seleccione un método de pago']" row>
              <v-radio label="Tarjeta de Crédito" value="credit-card" class="mr-4"></v-radio>
              <v-radio label="Sinpe" value="sinpe"></v-radio>
            </v-radio-group>
            <v-form-error v-if="!paymentMethod">Seleccione un método de pago</v-form-error>

            <v-expand-transition>
              <!-- Selección de tarjeta de crédito -->
              <div v-if="paymentMethod === 'credit-card'" class="mt-4">
                <v-btn color="secondary" text @click="toggleCreditCardMenu" class="mb-4" block>
                  <v-icon left>mdi-arrow-down</v-icon>
                  {{ isCreditCardMenuOpen ? 'Ocultar Tarjetas de Crédito' : 'Mostrar Tarjetas de Crédito' }}
                </v-btn>
                <v-expand-transition>
                  <div v-show="isCreditCardMenuOpen">
                    <v-row>
                      <v-col v-for="(card, index) in creditCards" :key="index" cols="12" class="mb-3">
                        <v-card class="mx-auto custom-card" width="100%" max-width="700" elevation="5" hover
                          @click="selectCreditCard(card)"
                          :class="{ 'selected-card': selectedCreditCard && selectedCreditCard.id === card.id }">
                          <v-card-item>
                            <v-icon color="primary" class="mr-2">mdi-credit-card-outline</v-icon>
                            <v-card-title>{{ card.name }}</v-card-title>
                          </v-card-item>
                          <v-divider></v-divider>
                          <v-card-text>
                            <p><strong>Nombre en la Tarjeta:</strong> {{ card.name }}</p>
                            <p><strong>Número de tarjeta:</strong> {{ card.number }}</p>
                            <p><strong>Vencimiento:</strong> {{ card.dateVenc }}</p>
                          </v-card-text>
                        </v-card>
                      </v-col>
                    </v-row>
                  </div>
                </v-expand-transition>
              </div>

              <!-- Subida de factura para Sinpe -->
              <div v-if="paymentMethod === 'sinpe'" class="mt-4">
                <v-card class="mx-auto custom-card mb-3" width="100%" max-width="700" elevation="5" hover>
                  <v-card-item>
                    <v-icon color="primary" class="mr-2">mdi-upload</v-icon>
                    <v-card-title>Sube la factura del Sinpe</v-card-title>
                  </v-card-item>
                  <v-divider></v-divider>
                  <v-card-text>
                    <v-file-input v-model="sinpeReceipt" label="Selecciona una imagen" accept="image/*"
                      :rules="[v => !!v || 'Sube la factura del Sinpe']" @change="updateSinpeReceipt"></v-file-input>
                  </v-card-text>
                </v-card>
              </div>
            </v-expand-transition>
          </v-form>

          <!-- Alerta de tarjeta o factura seleccionada -->
          <v-alert v-if="selectedCreditCard || sinpeReceipt" type="success" class="mt-4" elevation="2" border="left">
            <v-icon left color="green">mdi-check-circle</v-icon>
            <template v-if="paymentMethod === 'credit-card'">
              <strong>Tarjeta Seleccionada:</strong> {{ selectedCreditCard.name }} (**** {{
                selectedCreditCard.number }})
            </template>
            <template v-if="paymentMethod === 'sinpe' && sinpeReceipt">
              <strong>Factura Sinpe Seleccionada:</strong> {{ sinpeReceipt.name }}
            </template>
          </v-alert>
        </v-card-text>
      </v-card>

      <!-- PASO 4: Confirmación -->
      <v-card class="mb-5 pa-5 elevation-4" outlined>
        <v-card-title class="d-flex align-center">
          <v-icon large color="primary">mdi-check-circle</v-icon>
          <h2 class="ml-3">Paso 4: Confirmación</h2>
        </v-card-title>
        <v-divider class="my-4"></v-divider>
        <v-spacer></v-spacer>
        <v-btn color="success" @click="submitForm" max-width="300" block>
          <v-icon right>mdi-check-circle</v-icon>
          Confirmar
        </v-btn>
      </v-card>

    </v-container>
  </v-main>
</template>

<script>
import axios from 'axios';
import { API_URL, URL } from '@/main';
import { getToken } from '@/helpers/auth';

export default {
  data() {
    return {
      userId: '',
      paymentMethod: '',
      shoppingCartId: '',
      cartProducts: [],
      directions: [],
      creditCards: [],
      selectedCreditCard: null,
      sinpeReceipt: null,
      isAddressOpen: false,
      selectedAddress: null,
      isCreditCardMenuOpen: false,
    };
  },

  created() {
    const user = JSON.parse(localStorage.getItem('user')) || {};
    this.userId = user.id;
    this.GetShoppingCart();
    this.GetDirectionsOfUser();
    this.GetCreditCardsOfUser();
  },

  methods: {
    GetShoppingCart() {
      axios
        .get(`${API_URL}/ShoppingCart`, { params: { userId: this.userId } })
        .then((response) => {
          const shoppingCart = response.data;
          this.shoppingCartId = shoppingCart.id;
          this.cartProducts = shoppingCart.cartProducts.map((item) => ({
            ...item,
            subtotal: parseFloat(item.subtotal).toFixed(2),
            deliveryDate: item.product.type === 'Perishable' ? '' : this.getNextFriday(),
            imageURL: URL + "/" + item.product.imagesURLs[0],
            isDialogOpen: false,
          }));
        })
        .catch((error) => {
          console.error('Error fetching shopping cart', error);
        });
    },

    async GetCreditCardsOfUser() {
      const userId = JSON.parse(localStorage.getItem('user')).id;
      try {
        const result = await axios.get(
          `${API_URL}/CreditCard/User?userId=${encodeURIComponent(userId)}`
        );
        console.log(result.data);
        this.creditCards = result.data;
      } catch (error) {
        console.error('Error al obtener las tarjetas del usuario:', error);
        this.$swal.fire({
          title: 'Error',
          text: 'No se pudieron obtener las tarjetas de crédito.',
          icon: 'error',
          confirmButtonText: 'Ok',
        });
      }
    },

    async GetDirectionsOfUser() {
      const token = getToken();
      const user = JSON.parse(localStorage.getItem('user'));
      try {
        if (!user || !user.id) {
          throw new Error('El usuario no tiene todos los campos requeridos');
        }
        const response = await axios.post(
          `${API_URL}/Direction/ObtainDirectionsFromUser`,
          user,
          { headers: { Authorization: `Bearer ${token}` } }
        );
        this.directions = response.data;
      } catch (error) {
        console.error('Error al obtener las direcciones del usuario:', error);
      }
    },

    allowedDates(date, weekDaysAvailable) {
      const day = new Date(date).getDay();
      const allowedDays = weekDaysAvailable.split('').map(Number);
      return allowedDays.includes(day);
    },

    getNextFriday() {
      const today = new Date();
      const dayOfWeek = today.getDay();
      const daysUntilFriday = (5 - dayOfWeek + 7) % 7 || 7;
      const nextFriday = new Date();
      nextFriday.setDate(today.getDate() + daysUntilFriday);
      const year = nextFriday.getFullYear();
      const month = String(nextFriday.getMonth() + 1).padStart(2, '0');
      const day = String(nextFriday.getDate()).padStart(2, '0');
      return `${year}-${month}-${day}`;
    },

    onDateChange(item) {
      if (item.deliveryDate) {
        item.deliveryDate = new Date(item.deliveryDate)
          .toISOString()
          .split('T')[0];
      }
    },

    openDialog(item) {
      item.isDialogOpen = true;
    },

    closeDialog(item) {
      item.isDialogOpen = false;
    },

    selectAddress(direction) {
      this.selectedAddress = direction;
      this.isAddressOpen = false;
    },

    toggleCreditCardMenu() {
      this.isCreditCardMenuOpen = !this.isCreditCardMenuOpen;
    },

    selectCreditCard(card) {
      this.selectedCreditCard = card;
      this.isCreditCardMenuOpen = false;
    },

    updateSinpeReceipt(file) {
      this.sinpeReceipt = file;
    },

    toggleAddressMenu() {
      this.isAddressOpen = !this.isAddressOpen;
    },

    submitForm() {
      const step1Valid = this.$refs.step1Form.validate();
      const step3Valid = this.$refs.step3Form.validate();
      if (!step1Valid || !step3Valid || !this.selectedAddress) {
        return;
      }
      const orderData = {
        userId: this.userId,
        shoppingCartId: this.shoppingCartId,
        products: this.cartProducts.map((item) => ({
          productId: item.product.id,
          quantity: item.quantity,
          deliveryDate: item.deliveryDate,
        })),
        deliveryAddressId: this.selectedAddress.id,
        paymentMethod: this.paymentMethod,
      };

      axios
        .post(`${API_URL}/Order/SubmitOrder`, orderData, {
          headers: { Authorization: `Bearer ${getToken()}` },
        })
        .then(() => {
          this.resetForm();
        })
        .catch((error) => {
          console.error('Error al enviar la orden:', error);
        });
    },

    getToday() {
      const today = new Date();
      const year = today.getFullYear();
      const month = String(today.getMonth() + 1).padStart(2, '0');
      const day = String(today.getDate()).padStart(2, '0');
      return `${year}-${month}-${day}`;
    },

    resetForm() {
      this.$refs.step1Form.reset();
      this.$refs.step3Form.reset();
      this.cartProducts.forEach((item) => {
        if (item.product.type === 'Perishable') {
          item.deliveryDate = '';
          item.isDialogOpen = false;
        }
      });
      this.selectedAddress = null;
      this.paymentMethod = '';
    },
  },
};
</script>

<style lang="scss" scoped>
.v-card-title {
  display: flex;
  align-items: center;
}

.ml-3 {
  margin-left: 1rem;
}

.mb-5 {
  margin-bottom: 2.5rem !important;
}

.mb-4 {
  margin-bottom: 1.5rem !important;
}

.pa-3 {
  padding: 1rem !important;
}

.pa-5 {
  padding: 3rem !important;
}

.text-h6 {
  font-weight: 500;
}

.product-image {
  width: 100%;
  height: auto;
  object-fit: contain;
  border-radius: 8px;
}

.v-alert {
  display: flex;
  align-items: center;
  width: 100%;
}

.v-alert .v-icon {
  margin-right: 0.5rem;
}

.v-btn {
  text-transform: none;
  width: 100%;
}

.custom-card {
  cursor: pointer;
  transition: box-shadow 0.3s ease;
}

.custom-card:hover {
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
}

.selected-card {
  border-left: 4px solid #4caf50;
  /* Verde para indicar selección */
}
</style>
