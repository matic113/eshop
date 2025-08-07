# Offers Feature

## Overview

The Offers feature allows administrators to create promotional offer cards that will be displayed to customers on mobile devices. The feature includes a form for creating offers and a live mobile preview.

## Features

### Form Components

- **Name Input**: Text input for the offer name
- **Description Textarea**: Multi-line text area for offer description
- **Cover Image Upload**: File upload with preview and validation
- **Preview Button**: Validates form and shows preview
- **Save Button**: Saves the offer (currently shows success message)

### Mobile Preview

- **Realistic Mobile Frame**: iPhone-style device mockup
- **Live Preview**: Updates in real-time as you type
- **Status Bar**: Realistic mobile status bar
- **Offer Card Display**: Shows how the offer will appear to customers

### Validation

- **Required Fields**: Name, description, and cover image are required
- **File Validation**: Only image files accepted (PNG, JPG, GIF)
- **File Size**: Maximum 5MB file size
- **Real-time Error Clearing**: Errors clear when user starts typing

## File Structure

```
src/
├── app/dashboard/offers/
│   └── page.tsx                    # Main offers page
├── components/dashboard/
│   └── create-offer-form.tsx       # Form and preview component
└── components/ui/
    └── textarea.tsx                # Textarea component
```

## Usage

1. Navigate to `/dashboard/offers` in the admin panel
2. Fill in the offer name and description
3. Upload a cover image (max 5MB)
4. Use the mobile preview to see how it will look
5. Click "Save" to create the offer

## Technical Implementation

### State Management

- Uses React `useState` for form data and errors
- File preview URLs are created using `URL.createObjectURL()`
- Proper cleanup of object URLs to prevent memory leaks

### Responsive Design

- Grid layout: 1 column on mobile, 2 columns on desktop
- Mobile preview is centered and responsive
- Form adapts to different screen sizes

### File Handling

- Client-side file validation
- Image preview without server upload
- Proper error handling for invalid files

### Styling

- Consistent with existing design system
- Uses Tailwind CSS classes
- Follows the same patterns as Products page
- Dark mode support

## Future Enhancements

- Server-side image upload integration
- Offer management (edit, delete, list)
- Offer scheduling and expiration dates
- Multiple offer templates
- Analytics for offer performance
- A/B testing capabilities

## Dependencies

- React 18+
- TypeScript
- Tailwind CSS
- Lucide React (for icons)
- Custom UI components (Button, Input, Card, etc.)
