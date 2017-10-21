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
    orderId: number;


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
            this.orderId = orderId;
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
        this.http.post(`${this.baseUrl}api/Order/AddOrder`, {CustomerName: customerName}).subscribe(result => 
            {
                this.orders = result.json() as Order[];
            }, 
            error => console.error(error));
    }

    addOrderLine(productId: number, quantity: number) {
        this.http.post(`${this.baseUrl}api/Order/AddProductLine`, 
        {OrderId: this.orderId, ProductId: productId, Quantity: quantity}).subscribe(result => 
            {
                this.orderLines = result.json() as OrderLine[];
                let totalPrice: number = 0;
                
                for(let orderLine of this.orderLines) {
                    for(let product of this.products) {
                        if(product.id === orderLine.productId){
                            totalPrice += orderLine.quantity * product.price;
                            break;
                        }
                    }
                }

                this.currentOrder.totalPrice = totalPrice;
            }, 
            error => console.error(error));
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