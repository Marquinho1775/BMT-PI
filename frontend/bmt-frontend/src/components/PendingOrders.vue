<template>
    <v-container>
        <v-card elevation="4" height="600px">
            <v-card-title class="text-h6 mb-0">
                <v-icon class="mr-2">mdi-package-variant-closed</v-icon>
                Ordenes pendientes
            </v-card-title>
            <v-virtual-scroll class="component-scroller" :items="orders"
                v-if="orders && orders.length" :item-height="70">
                <template v-slot:default="{ item }">
                    <v-list-item :key="item.id" @click="openOrderDialog(item)">
                        <v-list-item-content>
                            <v-list-item-title>{{ item.order.orderId
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
                            selectedOrder?.order.orderId }}
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
                                <li v-for="(product, index) in selectedOrder.products"
                                    :key="index">
                                    {{ product.productName }} -
                                    Cantidad: {{
                                        product.quantity }} - Precio:
                                    ${{ product.productsCost }}
                                </li>
                            </ul>
                        </div>
                    </v-card-text>
                    <v-card-actions>
                        <v-btn v-if="selectedOrder?.order.status === 0"
                            color="red" @click="cancelOrder">
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
</template>

<script>
export default {
    setup() {


        return {}
    }
}
</script>

<style lang="scss" scoped></style>