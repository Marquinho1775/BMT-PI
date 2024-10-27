<template>
	<v-card class="mx-auto" max-width="344" elevation="4">
		<v-carousel show-arrows="hover" height="200px" hide-delimiters>
			<v-carousel-item v-for="(image, index) in product.raw.imagesURLs" :key="index">
				<v-img :src="image" height="200px" aspect-ratio="16/9" cover></v-img>
			</v-carousel-item>
		</v-carousel>
		<v-card-title>{{ product.raw.name }}</v-card-title>
		<v-card-subtitle>₡ {{ product.raw.price }}</v-card-subtitle>

		<v-card-actions>
			<v-btn prepend-icon="mdi-plus" color="primary" @click="addToCart" text>Añadir al carrito</v-btn>
			<v-spacer></v-spacer>
			<v-btn :icon="isShow ? 'mdi-chevron-up' : 'mdi-chevron-down'" @click="toggleShow"></v-btn>
		</v-card-actions>

		<v-expand-transition>
			<div v-show="isShow">
				<v-divider></v-divider>
				<v-spacer></v-spacer>
				<v-chip-group>
					<v-chip v-for="(tag, index) in product.raw.tags" :key="index" class="mr-4">{{ tag }}</v-chip>
				</v-chip-group>
				<v-card-text>{{ product.raw.description }}</v-card-text>
			</div>
		</v-expand-transition>
	</v-card>
</template>

<script>
import axios from 'axios';
import { API_URL } from '@/main';

export default {
	name: 'ProductCard',
	props: {
		product: {
			type: Object,
			required: true,
		},
	},
	data() {
		return {
			isShow: false,
			shoppingCartId: '',
			productId: '',
		};
	},
	mounted() {
		this.shoppingCartId = localStorage.getItem('shoppingCartId');
	},
	methods: {
		toggleShow() {
			this.isShow = !this.isShow;
		},
		addToCart() {
			console.log('Shopping cart ID:', this.shoppingCartId);
			console.log('Adding product:', this.product.raw.id);
			this.productId = this.product.raw.id;
			let response = axios
        .put(API_URL + '/ShoppingCart/AddProductToCart?shoppingCartId=' + this.shoppingCartId + '&productId=' + this.productId, null)
        .catch((error) => {
          console.error('Error adding product to cart:', error);
        });
				if (response) {
					this.$swal.fire({
						title: 'Producto añadido',
						text: '¡El producto ha sido añadido al carrito correctamente!',
						icon: 'success',
						confirmButtonText: 'Ok',
					});
				}
		},
	},
};
</script>

<style scoped>
.v-card {
	margin-bottom: 16px;
}
</style>