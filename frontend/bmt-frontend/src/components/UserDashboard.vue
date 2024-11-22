<template>
    <v-row>
        <v-col :cols="9">
            <v-container>
                <productSearchGrid :products="products" />
            </v-container>
        </v-col>
        <v-col :cols="3">
            <v-row class="new align-center custom-header">
                <v-container>
                    <v-card elevation="4" height="450px">
                        <v-card-title class="text-h6 mb-0">
                            <v-icon class="mr-2">mdi-package-variant-closed</v-icon>
                            Ordenes pendientes
                        </v-card-title>

                        <v-virtual-scroll :items="orders" v-if="orders && orders.length" :item-height="70">
                            <template v-slot:default="{ item }">
                                <v-list-item :key="item.id" @click="openOrderDialog(item)">
                                    <v-list-item-content>
                                        <v-list-item-title>{{ item.order.orderId }}</v-list-item-title>
                                        <v-list-item-subtitle>{{ item.order.deliveryDate }}</v-list-item-subtitle>
                                    </v-list-item-content>
                                </v-list-item>
                                <v-divider></v-divider>
                            </template>
                        </v-virtual-scroll>

                        <!-- Mensaje si no hay órdenes -->
                        <p v-else>No hay órdenes disponibles.</p>

                        <!-- Dialogo para Detalles de la Orden -->
                        <v-dialog v-model="dialog" max-width="600px">
                            <v-card elevation="4">
                                <v-card-title class="text-h5">
                                    Detalles de la Orden #{{ selectedOrder?.order.orderId }}
                                </v-card-title>
                                <v-card-text>
                                    <div v-if="selectedOrder">
                                        <p><strong>Cliente:</strong> {{ selectedOrder.userName }}</p>
                                        <p><strong>Dirección: </strong>{{ selectedOrder.direction }}</p>
                                        <p><strong>Correo:</strong> {{ selectedOrder.userEmail }}</p>
                                        <p><strong>Fecha de Entrega:</strong> {{ selectedOrder.order.deliveryDate }}</p>
                                        <p><strong>Estado:</strong>
                                            {{
                                                selectedOrder.order.status === 0 ? 'No confirmado' :
                                                    selectedOrder.order.status === 1 ? 'Confirmado' :
                                                        selectedOrder.order.status === 2 ? 'Listo para envío' :
                                                            selectedOrder.order.status === 3 ? 'Shipping' :
                                                                selectedOrder.order.status === 4 ? 'Terminado' :
                                                                    selectedOrder.order.status === 5 ? 'Cancelado' :
                                                                        'Desconocido'
                                            }}
                                        </p>
                                        <h5>Productos:</h5>
                                        <ul>
                                            <li v-for="(product, index) in selectedOrder.products" :key="index">
                                                {{ product.productName }} - Cantidad: {{ product.quantity }} - Precio:
                                                ${{ product.productsCost }}
                                            </li>
                                        </ul>
                                    </div>
                                </v-card-text>
                                <v-card-actions>
                                    <v-btn v-if="selectedOrder?.order.status === 0" color="red" @click="cancelOrder">
                                        Cancelar Pedido
                                    </v-btn>
                                    <v-spacer></v-spacer>
                                    <v-btn text @click="dialog = false">
                                        Cerrar
                                    </v-btn>
                                </v-card-actions>
                            </v-card>
                        </v-dialog>
                    </v-card>
                </v-container>
            </v-row>
            <v-row class="new align-center custom-header">
                <!-- Lista de productos de ordenes -->
                <v-container>
                    <v-card elevation="4" height="450px">
                        <v-card-title class="text-h6 mb-0">
                            <v-icon class="mr-2">mdi-dolly</v-icon>
                            Compras recientes
                        </v-card-title>

                        <v-virtual-scroll :items="orderProducts" v-if="orderProducts && orderProducts.length"
                            :item-height="70">
                            <template v-slot:default="{ item }">
                                <v-list-item :key="item.id" @click="openProductDialog(item)">
                                    <v-list-item-content>
                                        <v-img v-if="item.imagesURLs && item.imagesURLs.length"
                                            :src="item.imagesURLs[0]" aspect-ratio="1" class="mr-2"></v-img>
                                        <v-list-item-title>{{ item.name }}</v-list-item-title>
                                        <v-list-item-subtitle>₡{{ item.price }}</v-list-item-subtitle>
                                    </v-list-item-content>
                                </v-list-item>
                                <v-divider></v-divider>
                            </template>
                        </v-virtual-scroll>
                    </v-card>
                </v-container>
                <v-dialog v-model="productDialog" max-width="600px">
                    <v-card>
                        <v-card-title class="text-h5">
                            Detalles del Producto
                        </v-card-title>
                        <v-card-text>
                            <div v-if="selectedProduct">
                                <v-carousel show-arrows="hover" hide-delimiters>
                                    <v-carousel-item v-for="(image, index) in selectedProduct.imagesURLs" :key="index">
                                        <v-img :src="image" aspect-ratio="16/9" cover></v-img>
                                    </v-carousel-item>
                                </v-carousel>
                                <p><strong>Nombre: </strong>{{ selectedProduct.name }}</p>
                                <p><strong>Descripción:</strong> {{ selectedProduct.description }} </p>
                                <p><strong>Emprendimiento:</strong> {{ selectedProduct.enterpriseName }} </p>
                                <p><strong>Precio:</strong> ₡{{ selectedProduct.price }}</p>
                                <v-chip-group>
                                    <v-chip v-for="(tag, index) in selectedProduct.tags" :key="index" class="mr-4">{{
                                        tag
                                    }}</v-chip>
                                </v-chip-group>
                            </div>
                        </v-card-text>
                        <v-card-actions>
                            <v-btn prepend-icon="mdi-plus" color="primary" @click="addToCart" text>Añadir al
                                carrito</v-btn>

                            <v-spacer></v-spacer>
                            <v-btn text @click="productDialog = false">
                                Cerrar
                            </v-btn>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
            </v-row>
        </v-col>
    </v-row>
</template>


<script>
import axios from 'axios';
import { API_URL, URL } from '@/main.js';
import { getUser } from '@/helpers/auth';

export default {
    name: 'UserDashboard',
    data() {
        return {
            userId: '',
            products: [],
            orders: [],
            orderProducts: [],
            dialog: false,
            selectedOrder: null,
            productDialog: false,
            selectedProduct: null,
            shoppingCartId: '',
        };
    },
    created() {
        this.userId = getUser().id;
        this.getProducts();
        this.getOrders();
        this.getOrderProducts();
        this.shoppingCartId = localStorage.getItem('shoppingCartId');
    },
    methods: {

        async getOrderProducts() {
            try {
                const response = await axios.get(`${API_URL}/User/GetOrderProducts`, {
                    params: { userId: this.userId },
                });
                this.orderProducts = response.data.data;
            } catch (error) {
                console.error('Error fetching order products:', error);
            }
        },
        async getOrders() {
            try {
                console.log("Fetching orders for user ID:", this.userId);
                const response = await axios.get(`${API_URL}/User/GetInProgessOrder`, {
                    params: { userId: this.userId },
                });

                // Depura la respuesta para verificar que es lo que estás recibiendo
                console.log("Respuesta de getOrders:", response.data);

                // Asegúrate de que la estructura de datos sea la esperada
                this.orders = Array.isArray(response.data.data) ? response.data.data : [];
                console.log("Órdenes asignadas:", this.orders);

            } catch (error) {
                console.error("Error fetching orders:", error);
                this.orders = []; // Inicializa como un array vacío en caso de error
            }
        },
        async getProducts() {
            try {
                const response = await axios.get(`${API_URL}/Product/GetProductsDetails`);
                this.products = response.data.data;
                this.URLImage();
            } catch (error) {
                console.error('Error fetching products:', error);
            }
        },
        URLImage() {
            this.products.forEach(product => {
                if (Array.isArray(product.imagesURLs)) {
                    product.imagesURLs = product.imagesURLs.map(image => `${URL}${image}`);
                } else {
                    console.warn(`El producto con ID ${product.id} no tiene una propiedad imagesURLs válida.`);
                }
            });
        },
        openOrderDialog(orderWrapper) {
            this.selectedOrder = orderWrapper;
            this.dialog = true;
        },
        openProductDialog(item) {
            console.log('Producto seleccionado:', item);
            this.selectedProduct = item;
            this.productDialog = true;
        },


        cancelOrder() {
            if (confirm("¿Estás seguro de que quieres rechazar este pedido?")) {
                this.denyOrder(this.selectedOrder.order.orderId);
                this.dialog = false;
            }
        },
        async denyOrder(orderId) {
            try {
                const response = await axios.put(`${API_URL}/User/DenyOrder`, null, {
                    params: { orderID: orderId },
                });

                if (response.data.success) {
                    console.log(`Orden ${orderId} rechazada exitosamente`);
                    this.orders = this.orders.filter((orderWrapper) => orderWrapper.order.orderId !== orderId);
                    alert(`Orden ${orderId} rechazada exitosamente.`);
                } else {
                    console.error(`Fallo al rechazar la orden ${orderId}: ${response.data.Message}`);
                    alert(`Error: ${response.data.Message}`);
                }
            } catch (error) {
                console.error("Error al rechazar la orden:", error);
                alert("Ocurrió un error al rechazar la orden.");
            }
        },
        async addToCart() {
            if (!this.selectedProduct) {
                console.error('No hay un producto seleccionado.');
                return;
            }

            try {
                const productId = this.selectedProduct.id; // Usar el id del producto seleccionado
                const response = await axios.put(
                    `${API_URL}/ShoppingCart/AddProductToCart`,
                    null,
                    {
                        params: {
                            shoppingCartId: this.shoppingCartId,
                            productId,
                        },
                    }
                );

                if (response.data === "ProductExists") {
                    this.$swal.fire({
                        title: 'Ya está en el carrito',
                        text: 'Este producto ya está en el carrito',
                        icon: 'error',
                        confirmButtonText: 'Ok',
                    });
                } else {
                    this.$swal.fire({
                        title: 'Producto añadido',
                        text: 'El producto se ha añadido al carrito',
                        icon: 'success',
                        confirmButtonText: 'Ok',
                    });
                }
            } catch (error) {
                console.error('Error adding product to cart:', error);
                this.$swal.fire({
                    title: 'Error',
                    text: 'Ocurrió un error al añadir el producto al carrito',
                    icon: 'error',
                    confirmButtonText: 'Ok',
                });
            }
        }

    },
};
</script>


<style lang="scss" scoped>
.order-scroller {
    max-height: 600px;
    overflow-y: auto;
}

.new {
    margin-top: 10px;
    margin-right: 50px;
}
</style>