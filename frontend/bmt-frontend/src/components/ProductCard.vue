<template>
	<v-card class="mx-auto" max-width="344" elevation="4">
		<v-carousel show-arrows="hover" height="200px" hide-delimiters>
			<v-carousel-item v-for="(image, index) in product.imagesURLs" :key="index">
				<v-img :src="imagesURLBase + image" height="200px" aspect-ratio="16/9" cover></v-img>
			</v-carousel-item>
		</v-carousel>
		<v-row>
			<v-col>
				<v-card-title>{{ product.name }}</v-card-title>
				<v-card-subtitle>₡ {{ product.price }}</v-card-subtitle>
			</v-col>
			<v-col class="text-right">
				<div v-if="product.type === 'Perishable'">
					<v-chip v-for="(day, index) in formatWeekDays(product.weekDaysAvailable)" :key="index"
						color="info" size="small" class="white--text">
						{{ day }}
					</v-chip>
				</div>
				<div v-else>
					<div v-if="product.stock > 0">
						<v-chip color="success" class="white--text">Disponible</v-chip>
					</div>
					<div v-else>
						<v-chip color="error" class="white--text">Agotado</v-chip>
					</div>
				</div>
			</v-col>
		</v-row>

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
					<v-chip v-for="(tag, index) in product.tags" :key="index" class="mr-4">{{ tag }}</v-chip>
				</v-chip-group>
				<v-card-text>
					Descripción:
					<br />
					{{ product.description }}
					<br /><br />
					Emprendimiento:
					<br />
					{{ product.enterpriseName }}
				</v-card-text>
			</div>
		</v-expand-transition>
	</v-card>
</template>

<script>

import axios from 'axios';
import { API_URL, URL } from '@/main';

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
			imagesURLBase: URL,
			isShow: false,
			shoppingCartId: '',
			productId: '',
		};
	},
	mounted() {
		this.shoppingCartId = localStorage.getItem('shoppingCartId');
	},
	methods: {
		formatWeekDays(weekDays) {
			const daysMapping = {
				'0': 'Domingo',
				'1': 'Lunes',
				'2': 'Martes',
				'3': 'Miércoles',
				'4': 'Jueves',
				'5': 'Viernes',
				'6': 'Sábado',
			};
			const days = weekDays.split('');
			return days.map((day) => daysMapping[day]).filter(Boolean);
		},

		toggleShow() {
			this.isShow = !this.isShow;
		},
		async addToCart() {
			this.productId = this.product.id;
			let response = await axios
				.put(API_URL + '/ShoppingCart/AddProductToCart?shoppingCartId=' + this.shoppingCartId + '&productId=' + this.productId, null)
				.catch((error) => {
					console.error('Error adding product to cart:', error);
				});
			if (response.data === "Success") {
				this.$swal.fire({
					title: 'Producto añadido',
					text: 'El producto se ha añadido al carrito',
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

.v-chip {
	margin-top: 8px;
	margin-left: 8px;
	margin-right: 8px;
}

white--text {
	margin-top: 8px;
}
</style>