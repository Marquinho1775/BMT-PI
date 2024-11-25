<template>
    <v-row>
        <v-col :cols="9">
            <v-container>
                <productGrid :products="products" />
            </v-container>
        </v-col>
        <v-col :cols="3">
            <v-row class="new align-center custom-header">
                <v-container>
                    <pending-orders></pending-orders>
                </v-container>
            </v-row>
            <v-row class="new align-center custom-header">
                <!-- Lista de productos de ordenes -->
                <v-container>
                    <v-card elevation="4" height="600px">
                        <v-card-title class="text-h6 mb-0">
                            <v-icon class="mr-2">mdi-dolly</v-icon>
                            Compras recientes
                        </v-card-title>
                        <v-virtual-scroll class="component-scroller"
                            :items="orderProducts"
                            v-if="orderProducts && orderProducts.length"
                            :item-height="70">
                            <template v-slot:default="{ item }">
                                <v-list-item :key="item.id"
                                    @click="openProductDialog(item)">
                                    <v-list-item-content>
                                        <v-img
                                            v-if="item.imagesURLs && item.imagesURLs.length"
                                            :src="imagesURLBase + item.imagesURLs[0]"
                                            height="100" width="50"></v-img>
                                        <v-list-item-title>{{ item.name
                                            }}</v-list-item-title>
                                        <v-list-item-subtitle>₡{{ item.price
                                            }}</v-list-item-subtitle>
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
                                    <v-carousel-item
                                        v-for="(image, index) in selectedProduct.imagesURLs"
                                        :key="index">
                                        <v-img :src="imagesURLBase + image"
                                            aspect-ratio="16/9" cover></v-img>
                                    </v-carousel-item>
                                </v-carousel>
                                <p><strong>Nombre: </strong>{{
                                    selectedProduct.name }}</p>
                                <p><strong>Descripción:</strong> {{
                                    selectedProduct.description }} </p>
                                <p><strong>Emprendimiento:</strong> {{
                                    selectedProduct.enterpriseName }} </p>
                                <p><strong>Precio:</strong> ₡{{
                                    selectedProduct.price }}</p>
                                <v-chip-group>
                                    <v-chip
                                        v-for="(tag, index) in selectedProduct.tags"
                                        :key="index" class="mr-4">{{
                                            tag
                                        }}</v-chip>
                                </v-chip-group>
                            </div>
                        </v-card-text>
                        <v-card-actions>
                            <v-btn prepend-icon="mdi-plus" color="primary"
                                @click="addToCart" text>Añadir al
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
            imagesURLBase: URL,
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
                const response = await axios.get(`${API_URL}/User/GetInProgessOrder`, {
                    params: { userId: this.userId },
                });

                // Depura la respuesta para verificar que es lo que estás recibiendo

                // Asegúrate de que la estructura de datos sea la esperada
                this.orders = Array.isArray(response.data.data) ? response.data.data : [];

            } catch (error) {
                console.error("Error fetching orders:", error);
                this.orders = []; // Inicializa como un array vacío en caso de error
            }
        },
        async getProducts() {
            try {
                const response = await axios.get(`${API_URL}/Product/GetProductsDetails`);
                this.products = response.data.data;
            } catch (error) {
                console.error('Error fetching products:', error);
            }
        },

        openOrderDialog(orderWrapper) {
            this.selectedOrder = orderWrapper;
            this.dialog = true;
        },
        openProductDialog(item) {
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
.component-scroller {
    max-height: 550px;
    overflow-y: auto;
}

.new {
    margin-top: 9px;
    margin-right: 50px;
}
</style>