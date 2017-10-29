import { Component, OnInit } from '@angular/core';
import { Order, OrderLine, Product} from './entities';
import { OrderService } from './order.service';


@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css'],
    providers: [OrderService]
})
export class HomeComponent implements OnInit 
{
    orders: Order[];
    orderLines: OrderLine[];
    products: Product[];
    currentOrderId: number;
    searchMask: string;
    controlsEnabled: boolean;
    errorMsg: string;
    

    constructor(private orderService: OrderService)
    {
    }

    ngOnInit()
    {
        this.orderService.getErrorOccuredEmitter().subscribe((err: Error) => {
            this.handleError();
        });

        this.orderService.getOrders('').subscribe(result => 
        {
            this.initOrders(result);
            this.orderService.getProducts().subscribe(result => 
            {
                this.products = result;
                this.controlsEnabled = true;
            }, error => this.handleError());
        }, error => this.handleError());        
    }

    performSearch(searchMask: string)
    {
        this.controlsEnabled = false;
        this.orderService.getOrders(searchMask).subscribe(result => 
        {
            this.searchMask = searchMask;
            this.initOrders(result);
            this.controlsEnabled = true;
        }, error => this.handleError());
    }

    addOrder(customerName: string)
    {
        this.controlsEnabled = false;
        this.orderService.addOrder(customerName, this.searchMask).subscribe(result => 
        {
            this.orders = result;
            this.validateCurrentOrderId();
            this.controlsEnabled = true;
        }, error => this.handleError());
    }

    deleteOrder(orderId: number)
    {
        this.controlsEnabled = false;
        this.orderService.deleteOrder(orderId, this.searchMask).subscribe(result => 
        {
            this.orders = result;
            this.validateCurrentOrderId();
            this.controlsEnabled = true;
        }, error => this.handleError());
    }
    
    editOrder(orderId: number)
    {
        this.controlsEnabled = false;
        this.orderService.getOrderLines(orderId).subscribe(result => 
        {
            this.orderLines = result;
            this.currentOrderId = orderId;
            this.controlsEnabled = true;
        }, error => this.handleError());
    }

    addOrderLine(productId: number, quantity: number) 
    {
        if(quantity < 0)
        {
            this.errorMsg = "Quantity should be positive number";
            return;
        }
        this.controlsEnabled = false;
        let order: Order  = this.getOrder(this.currentOrderId);
        this.orderService.addOrderLine(this.currentOrderId, productId, quantity).subscribe(result => 
        {
            this.orderLines = result;
            order.totalPrice = this.calculateTotalPrice(result);
            this.controlsEnabled = true;
        }, error => this.handleError());
    }

    deleteOrderLine(orderLineId: number)
    {
        this.controlsEnabled = false;
        let order: Order  = this.getOrder(this.currentOrderId);
        this.orderService.deleteOrderLine(this.currentOrderId, orderLineId).subscribe(result => 
        {
            this.orderLines = result;
            order.totalPrice = this.calculateTotalPrice(result);
            this.controlsEnabled = true;
        }, error => this.handleError());
    }

    private handleError(){
        this.errorMsg = "Unexpected error occured. Please reload the page";
        this.controlsEnabled = false;
    }

    private initOrders(orders: Order[])
    {
        this.orders = orders;
        this.validateCurrentOrderId();
    }

    private validateCurrentOrderId()
    {
        if(this.currentOrderId < 0)
        {
            return;
        }

        let index = this.orders.findIndex(order => order.id === this.currentOrderId);

        if(index  < 0)
        {
            this.currentOrderId = -1;
            this.orderLines = [];
        }
    }

    private getOrder(orderId: number) : Order 
    {
        let order: Order | undefined = this.orders.find(order => order.id === orderId);
        if(order === undefined)
        {
            throw new Error(`Can not find order with id - ${orderId}`);
        }
        return order;
    }

    private calculateTotalPrice(orderLines: OrderLine[]): number 
    {
        let totalPrice: number = 0;
        for(let orderLine of this.orderLines) 
        {
            let product = this.products.find(pr => pr.id === orderLine.productId);
            if(product === undefined)
            {
                throw new Error(`Can not find product with id - ${orderLine.productId}`);
            }
            totalPrice += orderLine.quantity * product.price;

        }
        return totalPrice;
    }

}