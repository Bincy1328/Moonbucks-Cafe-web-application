let itemList = document.querySelector('.items');
let cart = document.querySelector('.cart');
let cartList = document.querySelector('.cart-list');
let total = document.querySelector('.total');
let tax = document.querySelector('.tax');
let subtotal = document.querySelector('.subtotal');


let items = [
    {
        id: 1,
        Name: 'Expresso Coffee',
        image: '../content/img/expresso.jpeg',
        Price: 50
    },
    {
        id: 2,
        Name: 'Bubble coffee',
        image: '../content/img/bubble coffee.jpeg',
        Price: 90
    },
    {
        id: 3,
        Name: 'Herbal coffee',
        image: '../content/img/herbal.jpeg',
        Price: 80
    },
    {
        id: 4,
        Name: 'Black coffee',
        image: '../content/img/blackcoffee.jpeg',
        Price: 50
    },
    {
        id: 5,
        Name: 'Chocolate coffee',
        image: '../content/img/chocolatecoffee.jpeg',
        Price: 60
    },
    {
        id: 6,
        Name: 'Milkshake',
        image: '../content/img/milkshack.jpeg',
        Price: 90
    },
    {
        id: 7,
        Name: 'french fries',
        image: '../content/img/french fries.jpeg',
        Price: 90
    },
    {
        id: 8,
        Name: 'eggpuff',
        image: '../content/img/eggpuff.jpeg',
        Price: 30
    },
    {
        id: 9,
        Name: 'breadomellete',
        image: '../content/img/breadomellete.jpeg',
        Price: 40
    }
]

function initItem() {
    items.forEach((value, key) => {
        let card = document.createElement('div');
        card.classList.add('card');
        card.setAttribute('style', 'width: 13rem;');
        card.innerHTML = `
            <img src="${value.image}" class="card-img-top" alt="..." style="height: 11rem;">
            <div class="card-body">
                <h5 class="card-title text-center">${value.Name}</h5>
                <p class="card-text text-center">Price: ${value.Price}</p>
                <button class="add-to-cart btn btn-dark form-control" onclick="addToCart(${key})">Add to Cart</button>
            </div>`;
        itemList.appendChild(card);
    });
}

initItem();

let cartLists = [];

function addToCart(key) {
    if (cartLists[key] == null) {
        cartLists[key] = JSON.parse(JSON.stringify(items[key]));
        cartLists[key].Quantity = 1;
    }
    reloadCart();
}

function reloadCart() {
    cartList.innerHTML = '';
    let totalPrice = 0;
    cartLists.forEach((value, key) => {
        totalPrice = totalPrice + value.Price;

        if (value != null) {
            let listItem = document.createElement('li');
            listItem.setAttribute('class', 'list-group-item');
            listItem.innerHTML = `
                <div><img src="${value.image}" style="width: 80px"/></div>
                <div><h5 class="mt-1 cartname">${value.Name}</h5></div>
                <div><h6 class="mt-2 cardprice">${value.Price.toLocaleString()}</h6></div>
                <div>
                    <button onclick="changeQuantity(${key}, ${value.Quantity - 1})">-</button>
                    <div class="count m-2 cartcount">${value.Quantity}</div>
                    <button onclick="changeQuantity(${key}, ${value.Quantity + 1})">+</button>
                </div>`;
            cartList.appendChild(listItem);
        }
    });

    // Calculate subtotal, tax, and total
    subtotal.innerText = totalPrice.toLocaleString();
    tax.innerText = (totalPrice * 0.12).toLocaleString(); // Assuming 12% tax
    total.innerText = (totalPrice + parseFloat(tax.innerText)).toLocaleString();

    quantity.innerText = count;
}

function changeQuantity(key, quantity) {
    if (quantity == 0) {
        delete cartLists[key];
    } else {
        cartLists[key].Quantity = quantity;
        cartLists[key].Price = quantity * items[key].Price;
    }
    reloadCart();
}

function clearCart() {
    cartLists = [];
    reloadCart();
}