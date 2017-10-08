import { Component, Inject } from '@angular/core';
import { Http, Response } from '@angular/http';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent {
    
    private baseUrl: string;
    orders: Order[];
    orderLines: OrderLine[];
    products: Product[];
    currentOrder: Order;

    constructor(private http: Http, @Inject('BASE_URL') baseUrl: string){
        this.baseUrl =  baseUrl;
        this.performSearch('');

        this.http.get(`${this.baseUrl}api/Order/Product`).subscribe(result => {
            this.products = result.json() as Product[];
            }, error => console.error(error));
    }

    performSearch(searchTerm: string){
        console.log(`User entered: ${searchTerm}`);
        this.http.get(`${this.baseUrl}api/Order/Search?mask=${searchTerm}`).subscribe(result => {
            this.orders = result.json() as Order[];
        }, error => console.error(error));
    }
    
    editOrder(orderId: number){
        this.http.get(`${this.baseUrl}api/Order/OrderLine?orderId=${orderId}`).subscribe(result => {
            this.orderLines = result.json() as OrderLine[];
            let index = this.orders.findIndex(order => order.id === orderId);
            this.currentOrder = this.orders[index];
        }, error => console.error(error));
    }

    deleteOrder(orderId: number){
        let index = this.orders.findIndex(order => order.id === orderId);
        this.orders.splice(index, 1);
    }


    deleteOrderLine(orderLineId: number){
        let index = this.orderLines.findIndex(orderLine => orderLine.id === orderLineId);
        this.orderLines.splice(index, 1);
    }

    addOrder(customerName: string){
        this. orders.push({id:1, customerName:customerName, totalPrice:24});
    }

    addOrderLine(productId: number, quantity: number){
        let index = this.products.findIndex(prod => prod.id === productId);
        if(index < 0)
        {
            return;
        }
        let product = this.products[index];
        this.orderLines.push( {id: 1, orderId: 1, productId: product.id, productPrice: product.price, quantity: quantity, productName: product.name});
    }
    
}

interface Order {
    id: number;
    customerName: string;
    totalPrice: number;
}

class OrderLine {
    id: number;
    orderId: number;
    productId: number;
    productName: string;
    productPrice: number;
    quantity: number;
}

class Product {
    id: number;
    name: string;
    price: number;
}