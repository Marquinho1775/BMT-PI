<template>
  <v-app class="d-flex flex-column">
    <AppHeader />
    <v-main class="flex-grow-1" style="overflow-x: hidden;">
      <h1 class="title1">Pedidos por revisar</h1>
      <template v-if="items && items.length > 0">
      <v-virtual-scroll :items="items" height="100%">
        <template v-slot:default="{ item }">
          <v-row class="order-card mb-4 p-1 bg-light-grey rounded" justify="space-between">
            <v-col style="padding-left: 5rem">
              <h4>{{ item.UserName }}</h4>
              <p>{{ item.Direction }} - {{ item.OtherSigns }}</p>
              <ul>
                <li v-for="(products, enterpriseName) in groupProductsByEnterprise(item.Products)" :key="enterpriseName">
                  <strong>{{ enterpriseName }}</strong>
                  <ul class="product-list">
                    <li v-for="product in products" :key="product.ProductId" style="padding-bottom: 0.3rem; padding-top: 0.3rem;">
                      <span class="quantity-box">{{ product.Quantity }}</span> {{ product.ProductName }}
                    </li>
                  </ul>
                </li>
              </ul>
              <p>Peso: {{ item.Weight }} kg</p>
              <p>{{getTotalProductQuantity(item.Products)}} artículos • Costo: ₡{{ item.OrderCost.toFixed(2) }} + ₡{{ item.DeliveryFee.toFixed(2) }} de envío</p>
              <p>{{ item.OrderDate.toLocaleDateString() }}</p>
            </v-col>
            <v-col class="d-flex flex-column align-center justify-center" cols="auto">
              <v-btn size="x-large" class="mb-3 custom-btn" :style="{ backgroundColor: '#d0eda0', color: 'black' }">
                Aceptar pedido
              </v-btn>
              <v-btn size="x-large" class="custom-btn" :style="{ backgroundColor: '#9fc9fc', color: 'black' }">
                Rechazar pedido
              </v-btn>
            </v-col>
          </v-row>
        </template>
      </v-virtual-scroll>
    </template>
    </v-main>
    <AppFooter />
    <AppSidebar />
  </v-app>
</template>

<script>
export default {
  data() {
    return {
      orders: [
        {
          OrderId: "ORD001",
          OrderDate: new Date("2023-10-10"),
          OrderCost: 120.5,
          DeliveryFee: 15.0,
          Weight: 5.2,
          UserId: "USR123",
          UserName: "John Doe",
          Direction: "123 Main St",
          OtherSigns: "Near the park",
          Coordinates: "40.712776,-74.005974",
          Status: 1,
          Products: [
            { ProductId: "PRD001", ProductName: "Product A", Quantity: 2, EnterpriseName: "Enterprise 1" },
            { ProductId: "PRD002", ProductName: "Product B", Quantity: 1, EnterpriseName: "Enterprise 2" }
          ]
        },
        {
          OrderId: "ORD002",
          OrderDate: new Date("2023-11-12"),
          OrderCost: 200.75,
          DeliveryFee: 20.0,
          Weight: 7.3,
          UserId: "USR456",
          UserName: "Jane Smith",
          Direction: "456 Elm St",
          OtherSigns: "Opposite the school",
          Coordinates: "34.052235,-118.243683",
          Status: 2,
          Products: [
            { ProductId: "PRD003", ProductName: "Product C", Quantity: 3, EnterpriseName: "Enterprise 3" }
          ]
        },
        {
          OrderId: "ORD003",
          OrderDate: new Date("2024-05-12"),
          OrderCost: 200.75,
          DeliveryFee: 20.0,
          Weight: 7.3,
          UserId: "USR457",
          UserName: "David Villanueva",
          Direction: "456 Elm St",
          OtherSigns: "500 metros al norte",
          Coordinates: "34.052235,-118.243683",
          Status: 2,
          Products: [
            { ProductId: "PRD001", ProductName: "Product A", Quantity: 3, EnterpriseName: "Enterprise 3" },
            { ProductId: "PRD002", ProductName: "Product B", Quantity: 7, EnterpriseName: "Enterprise 3" },
            { ProductId: "PRD003", ProductName: "Product C", Quantity: 6, EnterpriseName: "Enterprise 1" },
            { ProductId: "PRD004", ProductName: "Product D", Quantity: 5, EnterpriseName: "Enterprise 10" },
            { ProductId: "PRD005", ProductName: "Product E", Quantity: 3, EnterpriseName: "Enterprise 5" },
            { ProductId: "PRD006", ProductName: "Product F", Quantity: 1, EnterpriseName: "Enterprise 2" },
            { ProductId: "PRD007", ProductName: "Product G", Quantity: 2, EnterpriseName: "Enterprise 1" },
            { ProductId: "PRD008", ProductName: "Product H", Quantity: 15, EnterpriseName: "Enterprise 16" }
          ]
        }
        // Agrega más órdenes si es necesario
      ]
    };
  },
  computed: {
    items() {
      return this.orders;
    }
  },
  methods: {
    getStatusText(status) {
      const statuses = ["No confirmado", "Confirmado", "Listo para envío", "Shipping", "Terminado", "Cancelado"];
      return statuses[status] || "Desconocido";
    },
    groupProductsByEnterprise(products) {
      return products.reduce((grouped, product) => {
        (grouped[product.EnterpriseName] = grouped[product.EnterpriseName] || []).push(product);
        return grouped;
      }, {});
    },
    getTotalProductQuantity(products) {
      return products.reduce((total, product) => total + product.Quantity, 0);
    }
  }
};
</script>

<style scoped>
.order-card {
  border-bottom: 2px solid #dddddd;
}

.custom-btn {
  min-width: 100%;
  margin-right: 80%;
  font-weight: 700;
}

.product-list {
  padding-left: 1rem;
  list-style-type: none;
  padding-bottom: 0.7rem;
}

.quantity-box {
  display: inline-block;
  width: 24px;
  height: 24px;
  line-height: 24px;
  text-align: center;
  border: 1px solid #ccc;
  border-radius: 4px;
  margin-right: 8px;
  font-weight: bold;
  background-color: #f9f9f9;
}

.title1 {
  text-align: left;
  padding: 1rem;
  padding-left: 2rem;
  font-weight: 700;
}

.order-card p,
.order-card h4 {
  margin: 0; /* Elimina el margen por defecto */
  padding: 0.1rem 0; /* Ajusta el padding para un espaciado más pequeño */
}

.order-card h4 {
  font-weight: 700;
}
</style>
