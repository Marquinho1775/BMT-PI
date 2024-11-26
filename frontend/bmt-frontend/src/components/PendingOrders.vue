<template>
    <v-card elevation="3" height="600px">
        <v-card-title class="text-h6 mb-0">
            <v-icon class="mr-2">mdi-package-variant-closed</v-icon>
            Ordenes pendientes
        </v-card-title>
        <v-virtual-scroll class="component-scroller" :items="orders" v-if="orders && orders.length" :item-height="70">
            <template v-slot:default="{ item }">
                <v-list-item :key="item.id" @click="openOrderDialog(item)">
                    <v-list-item-content>
                        <v-list-item-title>Número de orden: {{ item.order.numOrder
                            }}</v-list-item-title>
                        <v-list-item-subtitle>{{
                            item.order.deliveryDate
                            }}</v-list-item-subtitle>
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
                    Detalles de la Orden #{{
                        selectedOrder?.order.numOrder }}
                </v-card-title>
                <v-card-text>
                    <div v-if="selectedOrder">
                        <p><strong>Cliente:</strong> {{
                            selectedOrder.userName }}</p>
                        <p><strong>Dirección: </strong>{{
                            selectedOrder.direction }}</p>
                        <p><strong>Correo:</strong> {{
                            selectedOrder.userEmail }}</p>
                        <p><strong>Fecha de Entrega:</strong> {{
                            selectedOrder.order.deliveryDate }}
                        </p>
                        <p><strong>Estado:</strong>
                            {{
                                selectedOrder.order.status === 0 ?
                                    'No confirmado' :
                                    selectedOrder.order.status === 1 ?
                                        'Confirmado' :
                                        selectedOrder.order.status === 2 ?
                                            'Listo para envío' :
                                            selectedOrder.order.status === 3 ?
                                                'Shipping' :
                                                selectedOrder.order.status === 4 ?
                                                    'Terminado' :
                                                    selectedOrder.order.status === 5 ?
                                                        'Cancelado' :
                                                        'Desconocido'
                            }}
                        </p>
                        <h5>Productos:</h5>
                        <ul>
                            <li v-for="(product, index) in selectedOrder.products" :key="index">
                                {{ product.productName }} -
                                Cantidad: {{
                                    product.quantity }} - Precio:
                                ${{ product.productsCost }}
                            </li>
                        </ul>
                    </div>
                </v-card-text>
                <v-card-actions>
                    <v-btn v-if="selectedOrder?.order.status === 0 && (userRole === 'dev')" color="green" @click="AcceptOrder">
                        Aceptar Orden
                    </v-btn>

                    <v-btn v-if="selectedOrder?.order.status === 0 && (userRole === 'dev' || userRole === 'cli')" color="red" @click="cancelOrder">
                        Cancelar Orden
                    </v-btn>

                    <v-spacer></v-spacer>
                    <v-btn text @click="dialog = false">
                        Cerrar
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-card>
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
            userRole: '',
            products: [],
            orders: [],
            orderProducts: [],
            dialog: false,
            selectedOrder: null,
            productDialog: false,
            selectedProduct: null,
            enterpriseId: '',
        };
    },
    async created() {
        this.userId = getUser().id;
        this.userRole = getUser().role;
        if (this.userRole === 'cli') {
            await this.getUserOrders();
        } else if (this.userRole === 'emp') {
            this.enterpriseId = this.$route.params.id;
            await this.getEnterpriseOrders();
        } else {
            await this.getAllOrders();
        }
    },
    methods: {
        async getUserOrders() {
            try {
                const response = await axios.get(`${API_URL}/User/GetInProgessOrder`, {
                    params: { userId: this.userId },
                });
                this.orders = Array.isArray(response.data.data) ? response.data.data : [];

            } catch (error) {
                console.error("Error fetching orders:", error);
                this.orders = [];
            }
        },
        async getEnterpriseOrders() {
            const requestBody = {
                    fechaInicio: "1990-01-01",
                    fechaFin: new Date(new Date().getTime() + 24 * 60 * 60 * 1000).toISOString().split('T')[0],
                    statusInicial: 0,
                    statusFinal: 3,
                    enterpriseId: this.enterpriseId,
                };
            try {
                const response = await axios.post(`${API_URL}/Order/GetOrdersByDateAndStatus`, requestBody);
                console.log(response.data);
                this.orders = response.data;

            } catch (error) {
                console.error("Error fetching orders:", error);
                this.orders = [];
            }
        },
        async getAllOrders() {
            const requestBody = {
                    fechaInicio: "1990-01-01",
                    fechaFin: new Date(new Date().getTime() + 24 * 60 * 60 * 1000).toISOString().split('T')[0],
                    statusInicial: 0,
                    statusFinal: 3,
                };
            try {
                const response = await axios.post(`${API_URL}/Order/GetOrdersByDateAndStatus`, requestBody);
                this.orders = response.data;

            } catch (error) {
                console.error("Error fetching orders:", error);
                this.orders = [];
            }
        },
        openOrderDialog(orderWrapper) {
            this.selectedOrder = orderWrapper;
            this.dialog = true;
        },

        async AcceptOrder() {
            if (confirm("¿Estás seguro de que quieres aceptar este Orden?")) {
                this.acceptOrder(this.selectedOrder.order.orderId);
                this.dialog = false;
            }
        },

        async acceptOrder(orderId) {
            try {
                const response = await axios.put(`${API_URL}/Developer/ConfirmOrder`, null, {
                    params: { orderID: orderId },
                });

                if (response.status === 200) {
                    alert(`Orden ${orderId} aceptada exitosamente.`);
                    this.orders = this.orders.map((orderWrapper) => {
                        if (orderWrapper.order.orderId === orderId) {
                            orderWrapper.order.status = 1;
                        }
                        return orderWrapper;
                    });
                } else {
                    console.error(`Fallo al aceptar la orden ${orderId}: ${response.data.Message}`);
                    alert(`Error: ${response.data.Message}`);
                }
            } catch (error) {
                console.error("Error al aceptar la orden:", error);
                alert("Ocurrió un error al aceptar la orden.");
            }
        },

        cancelOrder() {
            if (confirm("¿Estás seguro de que quieres rechazar este Orden?")) {
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
    },

}
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