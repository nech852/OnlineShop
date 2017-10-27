export interface Order {
    id: number;
    customerName: string;
    totalPrice: number;
}

export class OrderLine {
    id: number;
    orderId: number;
    productId: number;
    productName: string;
    productPrice: number;
    quantity: number;
}

export class Product {
    id: number;
    name: string;
    price: number;
}